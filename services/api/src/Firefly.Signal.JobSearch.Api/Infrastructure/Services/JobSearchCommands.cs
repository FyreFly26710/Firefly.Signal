using System.Text.Json;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Application.Mappers;
using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public sealed class JobSearchCommands(
    JobSearchDbContext dbContext,
    IJobSearchProvider jobSearchProvider) : IJobSearchCommands
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

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

        return JobResponseMappers.ToDetailsResponse(job);
    }

    public async Task<JobDetailsResponse?> UpdateAsync(long id, UpdateJobRequest request, CancellationToken cancellationToken = default)
    {
        var job = await dbContext.Jobs.SingleOrDefaultAsync(existingJob => existingJob.Id == id, cancellationToken);
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
        return JobResponseMappers.ToDetailsResponse(job);
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
            JsonSerializer.Serialize(new { fileName, jobCount = payload.Jobs.Count }, JsonOptions),
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
            .Where(job => requestedIds.Contains(job.Id))
            .ToListAsync(cancellationToken);

        foreach (var job in jobs.Where(existingJob => !existingJob.IsHidden))
        {
            job.Hide();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var hiddenIds = jobs.Select(job => job.Id).ToArray();
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
            .Where(job => requestedIds.Contains(job.Id))
            .ToListAsync(cancellationToken);

        var notHiddenIds = jobs.Where(job => !job.IsHidden).Select(job => job.Id).ToArray();
        if (notHiddenIds.Length > 0)
        {
            var existingIds = jobs.Select(job => job.Id).ToHashSet();
            var unresolvedIds = requestedIds.Where(id => !existingIds.Contains(id)).ToArray();
            return new DeleteJobsResponse(0, [], unresolvedIds, notHiddenIds);
        }

        foreach (var job in jobs)
        {
            job.MarkDeleted();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var deletedIds = jobs.Select(job => job.Id).ToArray();
        var missingIds = requestedIds.Except(deletedIds).ToArray();
        return new DeleteJobsResponse(deletedIds.Length, deletedIds, missingIds, []);
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

    private sealed record ImportedJobsFile(
        DateTime? ExportedAtUtc,
        int? Count,
        IReadOnlyList<CreateJobRequest> Jobs);
}
