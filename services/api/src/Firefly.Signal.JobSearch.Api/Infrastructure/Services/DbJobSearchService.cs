using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.SharedKernel.Models;
using System.Text.Json;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public sealed class DbJobSearchService(
    JobSearchDbContext dbContext,
    IJobSearchProvider jobSearchProvider) : IJobSearchService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public async Task<JobDetailsResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await dbContext.Jobs
            .Where(x => x.Id == id)
            .Select(ToResponse())
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<Paged<JobDetailsResponse>> GetPageAsync(GetJobsPageRequest request, CancellationToken cancellationToken = default)
    {
        var pageIndex = Math.Max(request.PageIndex, 0);
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

        var query = ApplyFilters(dbContext.Jobs.AsQueryable(), request);

        var totalCount = await query.LongCountAsync(cancellationToken);
        var jobs = await query
            .OrderByDescending(x => x.PostedAtUtc)
            .ThenByDescending(x => x.Id)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(ToResponse())
            .ToListAsync(cancellationToken);

        return new Paged<JobDetailsResponse>(pageIndex, pageSize, totalCount, jobs);
    }

    public async Task<ExportJobsResponse> ExportAsync(ExportJobsRequest request, CancellationToken cancellationToken = default)
    {
        IQueryable<JobPosting> query;

        if (request.JobIds is { Count: > 0 })
        {
            var ids = request.JobIds.Distinct().ToArray();
            query = dbContext.Jobs.AsNoTracking().Where(x => ids.Contains(x.Id));
        }
        else
        {
            query = ApplyFilters(dbContext.Jobs.AsNoTracking(), new GetJobsPageRequest(
                0,
                int.MaxValue,
                request.Keyword,
                request.Company,
                request.Postcode,
                request.Location,
                request.SourceName,
                request.CategoryTag,
                request.IsHidden));
        }

        var jobs = await query
            .OrderByDescending(x => x.PostedAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(ToExportEntry())
            .ToListAsync(cancellationToken);

        return new ExportJobsResponse(DateTime.UtcNow, jobs.Count, jobs);
    }

    public async Task<JobDetailsResponse> CreateAsync(CreateJobRequest request, CancellationToken cancellationToken = default)
    {
        var job = JobPosting.Create(
            request.JobRefreshRunId,
            request.SourceName,
            request.SourceJobId,
            request.SourceAdReference,
            request.Title,
            request.Description,
            request.Summary,
            request.Url,
            request.Company,
            request.CompanyDisplayName,
            request.CompanyCanonicalName,
            request.Postcode,
            request.LocationName,
            request.LocationDisplayName,
            request.LocationAreaJson,
            request.Latitude,
            request.Longitude,
            request.CategoryTag,
            request.CategoryLabel,
            request.SalaryMin,
            request.SalaryMax,
            request.SalaryCurrency,
            request.SalaryIsPredicted,
            request.ContractTime,
            request.ContractType,
            request.IsFullTime,
            request.IsPartTime,
            request.IsPermanent,
            request.IsContract,
            request.IsRemote,
            request.PostedAtUtc,
            request.ImportedAtUtc,
            request.LastSeenAtUtc,
            request.IsHidden,
            request.RawPayloadJson);

        dbContext.Jobs.Add(job);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(job);
    }

    public async Task<JobDetailsResponse?> UpdateAsync(long id, UpdateJobRequest request, CancellationToken cancellationToken = default)
    {
        var job = await dbContext.Jobs.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (job is null)
        {
            return null;
        }

        job.Update(
            request.JobRefreshRunId,
            request.SourceName,
            request.SourceJobId,
            request.SourceAdReference,
            request.Title,
            request.Description,
            request.Summary,
            request.Url,
            request.Company,
            request.CompanyDisplayName,
            request.CompanyCanonicalName,
            request.Postcode,
            request.LocationName,
            request.LocationDisplayName,
            request.LocationAreaJson,
            request.Latitude,
            request.Longitude,
            request.CategoryTag,
            request.CategoryLabel,
            request.SalaryMin,
            request.SalaryMax,
            request.SalaryCurrency,
            request.SalaryIsPredicted,
            request.ContractTime,
            request.ContractType,
            request.IsFullTime,
            request.IsPartTime,
            request.IsPermanent,
            request.IsContract,
            request.IsRemote,
            request.PostedAtUtc,
            request.ImportedAtUtc,
            request.LastSeenAtUtc,
            request.IsHidden,
            request.RawPayloadJson);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(job);
    }

    public async Task<ImportJobsResponse> ImportFromProviderAsync(
        ImportJobsFromProviderRequest request,
        CancellationToken cancellationToken = default)
    {
        var providerRequest = new SearchJobsRequest(
            request.Where,
            request.Keyword,
            request.PageIndex,
            request.PageSize,
            request.Provider,
            request.ExcludedKeyword,
            request.DistanceKilometers,
            request.Category,
            request.SalaryMin,
            request.SalaryMax,
            MaxDaysOld: request.MaxDaysOld);

        var filtersJson = JsonSerializer.Serialize(request, JsonOptions);
        var refreshRun = JobRefreshRun.Start(
            request.Provider.ToString(),
            "gb",
            filtersJson,
            request.PageSize,
            requestedMaxPages: 1);

        dbContext.JobRefreshRuns.Add(refreshRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var providerResult = await jobSearchProvider.SearchAsync(providerRequest, cancellationToken);
            refreshRun.RecordFetchedPage(providerResult.Jobs.Count);

            var importedJobs = providerResult.Jobs
                .Select(job => CloneImportedJob(job, refreshRun.Id))
                .ToArray();

            dbContext.Jobs.AddRange(importedJobs);
            refreshRun.RecordInsertedJobs(importedJobs.Length);
            refreshRun.RecordHiddenJobs(importedJobs.Count(job => job.IsHidden));
            refreshRun.Complete();

            await dbContext.SaveChangesAsync(cancellationToken);

            return new ImportJobsResponse(refreshRun.Id, request.Provider.ToString(), importedJobs.Length, 0);
        }
        catch
        {
            refreshRun.RecordFailedItems(1);
            refreshRun.Fail("Provider import failed.");
            await dbContext.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    public async Task<ImportJobsResponse> ImportFromJsonAsync(
        Stream jsonStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(jsonStream);

        var payload = await JsonSerializer.DeserializeAsync<ImportedJobsFile>(
            jsonStream,
            JsonOptions,
            cancellationToken);

        if (payload?.Jobs is null || payload.Jobs.Count == 0)
        {
            throw new InvalidDataException($"The uploaded file '{fileName}' does not contain any jobs.");
        }

        var refreshRun = JobRefreshRun.Start(
            "json-upload",
            "gb",
            JsonSerializer.Serialize(new
            {
                fileName,
                jobCount = payload.Jobs.Count
            }, JsonOptions),
            payload.Jobs.Count,
            requestedMaxPages: 1);

        dbContext.JobRefreshRuns.Add(refreshRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            refreshRun.RecordFetchedPage(payload.Jobs.Count);

            var importedJobs = payload.Jobs
                .Select(job => CreateImportedJob(job, refreshRun.Id))
                .ToArray();

            dbContext.Jobs.AddRange(importedJobs);
            refreshRun.RecordInsertedJobs(importedJobs.Length);
            refreshRun.RecordHiddenJobs(importedJobs.Count(job => job.IsHidden));
            refreshRun.Complete();

            await dbContext.SaveChangesAsync(cancellationToken);

            return new ImportJobsResponse(refreshRun.Id, "json-upload", importedJobs.Length, 0);
        }
        catch
        {
            refreshRun.RecordFailedItems(1);
            refreshRun.Fail("JSON import failed.");
            await dbContext.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    public async Task<HideJobsResponse> HideAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return new HideJobsResponse(0, [], []);
        }

        var requestedIds = ids.Distinct().ToArray();
        var jobs = await dbContext.Jobs
            .Where(x => requestedIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        foreach (var job in jobs.Where(x => !x.IsHidden))
        {
            job.Hide();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var hiddenIds = jobs.Select(x => x.Id).ToArray();
        var missingIds = requestedIds.Except(hiddenIds).ToArray();
        return new HideJobsResponse(hiddenIds.Length, hiddenIds, missingIds);
    }

    public async Task<DeleteJobsResponse> DeleteAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return new DeleteJobsResponse(0, [], [], []);
        }

        var requestedIds = ids.Distinct().ToArray();
        var jobs = await dbContext.Jobs
            .Where(x => requestedIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var notHiddenIds = jobs.Where(x => !x.IsHidden).Select(x => x.Id).ToArray();
        if (notHiddenIds.Length > 0)
        {
            var existingIds = jobs.Select(x => x.Id).ToHashSet();
            var missingIds = requestedIds.Where(x => !existingIds.Contains(x)).ToArray();
            return new DeleteJobsResponse(0, [], missingIds, notHiddenIds);
        }

        foreach (var job in jobs)
        {
            job.MarkDeleted();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var deletedIds = jobs.Select(x => x.Id).ToArray();
        var missing = requestedIds.Except(deletedIds).ToArray();
        return new DeleteJobsResponse(deletedIds.Length, deletedIds, missing, []);
    }

    private static IQueryable<JobPosting> ApplyFilters(IQueryable<JobPosting> query, GetJobsPageRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(x =>
                x.Title.Contains(keyword) ||
                x.Description.Contains(keyword) ||
                x.Summary.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(request.Company))
        {
            var company = request.Company.Trim();
            query = query.Where(x =>
                x.Company.Contains(company) ||
                (x.CompanyDisplayName != null && x.CompanyDisplayName.Contains(company)) ||
                (x.CompanyCanonicalName != null && x.CompanyCanonicalName.Contains(company)));
        }

        if (!string.IsNullOrWhiteSpace(request.Postcode))
        {
            var postcode = request.Postcode.Trim().ToUpperInvariant();
            query = query.Where(x => x.Postcode.Contains(postcode));
        }

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            var location = request.Location.Trim();
            query = query.Where(x =>
                x.LocationName.Contains(location) ||
                (x.LocationDisplayName != null && x.LocationDisplayName.Contains(location)));
        }

        if (!string.IsNullOrWhiteSpace(request.SourceName))
        {
            var sourceName = request.SourceName.Trim();
            query = query.Where(x => x.SourceName.Contains(sourceName));
        }

        if (!string.IsNullOrWhiteSpace(request.CategoryTag))
        {
            var categoryTag = request.CategoryTag.Trim();
            query = query.Where(x => x.CategoryTag != null && x.CategoryTag.Contains(categoryTag));
        }

        if (request.IsHidden.HasValue)
        {
            query = query.Where(x => x.IsHidden == request.IsHidden.Value);
        }

        return query;
    }

    private static JobPosting CloneImportedJob(JobPosting source, long refreshRunId)
        => JobPosting.Create(
            refreshRunId,
            source.SourceName,
            source.SourceJobId,
            source.SourceAdReference,
            source.Title,
            source.Description,
            source.Summary,
            source.Url,
            source.Company,
            source.CompanyDisplayName,
            source.CompanyCanonicalName,
            source.Postcode,
            source.LocationName,
            source.LocationDisplayName,
            source.LocationAreaJson,
            source.Latitude,
            source.Longitude,
            source.CategoryTag,
            source.CategoryLabel,
            source.SalaryMin,
            source.SalaryMax,
            source.SalaryCurrency,
            source.SalaryIsPredicted,
            source.ContractTime,
            source.ContractType,
            source.IsFullTime,
            source.IsPartTime,
            source.IsPermanent,
            source.IsContract,
            source.IsRemote,
            source.PostedAtUtc,
            DateTime.UtcNow,
            DateTime.UtcNow,
            source.IsHidden,
            source.RawPayloadJson);

    private static JobPosting CreateImportedJob(CreateJobRequest request, long refreshRunId)
        => JobPosting.Create(
            refreshRunId,
            request.SourceName,
            request.SourceJobId,
            request.SourceAdReference,
            request.Title,
            request.Description,
            request.Summary,
            request.Url,
            request.Company,
            request.CompanyDisplayName,
            request.CompanyCanonicalName,
            request.Postcode,
            request.LocationName,
            request.LocationDisplayName,
            request.LocationAreaJson,
            request.Latitude,
            request.Longitude,
            request.CategoryTag,
            request.CategoryLabel,
            request.SalaryMin,
            request.SalaryMax,
            request.SalaryCurrency,
            request.SalaryIsPredicted,
            request.ContractTime,
            request.ContractType,
            request.IsFullTime,
            request.IsPartTime,
            request.IsPermanent,
            request.IsContract,
            request.IsRemote,
            request.PostedAtUtc,
            request.ImportedAtUtc == default ? DateTime.UtcNow : request.ImportedAtUtc,
            request.LastSeenAtUtc == default ? DateTime.UtcNow : request.LastSeenAtUtc,
            request.IsHidden,
            request.RawPayloadJson);

    private static Expression<Func<JobPosting, CreateJobRequest>> ToExportEntry()
        => job => new CreateJobRequest(
            job.JobRefreshRunId,
            job.SourceName,
            job.SourceJobId,
            job.SourceAdReference,
            job.Title,
            job.Description,
            job.Summary,
            job.Url,
            job.Company,
            job.CompanyDisplayName,
            job.CompanyCanonicalName,
            job.Postcode,
            job.LocationName,
            job.LocationDisplayName,
            job.LocationAreaJson,
            job.Latitude,
            job.Longitude,
            job.CategoryTag,
            job.CategoryLabel,
            job.SalaryMin,
            job.SalaryMax,
            job.SalaryCurrency,
            job.SalaryIsPredicted,
            job.ContractTime,
            job.ContractType,
            job.IsFullTime,
            job.IsPartTime,
            job.IsPermanent,
            job.IsContract,
            job.IsRemote,
            job.PostedAtUtc,
            job.ImportedAtUtc,
            job.LastSeenAtUtc,
            job.IsHidden,
            job.RawPayloadJson);

    private static Expression<Func<JobPosting, JobDetailsResponse>> ToResponse()
        => job => new JobDetailsResponse(
            job.Id,
            job.JobRefreshRunId,
            job.SourceName,
            job.SourceJobId,
            job.SourceAdReference,
            job.Title,
            job.Description,
            job.Summary,
            job.Url,
            job.Company,
            job.CompanyDisplayName,
            job.CompanyCanonicalName,
            job.Postcode,
            job.LocationName,
            job.LocationDisplayName,
            job.LocationAreaJson,
            job.Latitude,
            job.Longitude,
            job.CategoryTag,
            job.CategoryLabel,
            job.SalaryMin,
            job.SalaryMax,
            job.SalaryCurrency,
            job.SalaryIsPredicted,
            job.ContractTime,
            job.ContractType,
            job.IsFullTime,
            job.IsPartTime,
            job.IsPermanent,
            job.IsContract,
            job.IsRemote,
            job.PostedAtUtc,
            job.ImportedAtUtc,
            job.LastSeenAtUtc,
            job.IsHidden,
            job.RawPayloadJson);

    private static JobDetailsResponse Map(JobPosting job) => new(
        job.Id,
        job.JobRefreshRunId,
        job.SourceName,
        job.SourceJobId,
        job.SourceAdReference,
        job.Title,
        job.Description,
        job.Summary,
        job.Url,
        job.Company,
        job.CompanyDisplayName,
        job.CompanyCanonicalName,
        job.Postcode,
        job.LocationName,
        job.LocationDisplayName,
        job.LocationAreaJson,
        job.Latitude,
        job.Longitude,
        job.CategoryTag,
        job.CategoryLabel,
        job.SalaryMin,
        job.SalaryMax,
        job.SalaryCurrency,
        job.SalaryIsPredicted,
        job.ContractTime,
        job.ContractType,
        job.IsFullTime,
        job.IsPartTime,
        job.IsPermanent,
        job.IsContract,
        job.IsRemote,
        job.PostedAtUtc,
        job.ImportedAtUtc,
        job.LastSeenAtUtc,
        job.IsHidden,
        job.RawPayloadJson);

    private sealed record ImportedJobsFile(
        DateTime? ExportedAtUtc,
        int? Count,
        IReadOnlyList<CreateJobRequest> Jobs);
}
