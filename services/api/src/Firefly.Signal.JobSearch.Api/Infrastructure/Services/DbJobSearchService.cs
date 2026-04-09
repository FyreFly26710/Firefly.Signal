using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.SharedKernel.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public sealed class DbJobSearchService(JobSearchDbContext dbContext) : IJobSearchService
{
    public async Task<JobDetailsResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await dbContext.Jobs
            .Where(x => x.Id == id)
            .Select(ToResponse())
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<Paged<JobDetailsResponse>> GetPageAsync(GetJobsPageRequest request, CancellationToken cancellationToken = default)
    {
        var pageIndex = Math.Max(request.PageIndex, 0);
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

        var query = dbContext.Jobs.AsQueryable();

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
}
