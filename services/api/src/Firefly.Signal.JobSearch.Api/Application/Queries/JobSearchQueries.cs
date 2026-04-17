using Firefly.Signal.JobSearch.Application.Mappers;
using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Queries;

public sealed class JobSearchQueries(JobSearchDbContext dbContext) : IJobSearchQueries
{
    public async Task<JobDetailsResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await dbContext.Jobs
            .Where(job => job.Id == id)
            .Select(JobResponseMappers.ToDetailsResponse())
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<Paged<JobSearchResultResponse>> SearchPageAsync(
        GetJobsPageRequest request,
        long? userId,
        CancellationToken cancellationToken = default)
    {
        var pageIndex = Math.Max(request.PageIndex, 0);
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        var query = ApplyFilters(dbContext.Jobs.AsQueryable(), request with { IsHidden = false });
        var totalCount = await query.LongCountAsync(cancellationToken);

        List<JobSearchResultResponse> items;

        if (userId.HasValue)
        {
            var userStates = dbContext.UserJobStates.Where(state => state.UserAccountId == userId.Value);

            items = await (
                from job in query
                join state in userStates on job.Id equals state.JobPostingId into joined
                from state in joined.DefaultIfEmpty()
                orderby job.PostedAtUtc descending, job.Id descending
                select new JobSearchResultResponse(
                    Id: job.Id,
                    SourceJobId: job.SourceJobId,
                    Title: job.Title,
                    Summary: job.Summary,
                    Url: job.Url,
                    Company: job.Company,
                    CompanyDisplayName: job.CompanyDisplayName,
                    LocationName: job.LocationName,
                    LocationDisplayName: job.LocationDisplayName,
                    IsRemote: job.IsRemote,
                    IsHidden: job.IsHidden,
                    SalaryMin: job.SalaryMin,
                    SalaryMax: job.SalaryMax,
                    SalaryCurrency: job.SalaryCurrency,
                    ContractType: job.ContractType,
                    ContractTime: job.ContractTime,
                    IsFullTime: job.IsFullTime,
                    IsPartTime: job.IsPartTime,
                    IsPermanent: job.IsPermanent,
                    IsContract: job.IsContract,
                    SourceName: job.SourceName,
                    PostedAtUtc: job.PostedAtUtc,
                    IsSaved: state != null && state.IsSaved,
                    IsUserHidden: state != null && state.IsHidden))
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        else
        {
            items = await query
                .OrderByDescending(job => job.PostedAtUtc)
                .ThenByDescending(job => job.Id)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(job => new JobSearchResultResponse(
                    Id: job.Id,
                    SourceJobId: job.SourceJobId,
                    Title: job.Title,
                    Summary: job.Summary,
                    Url: job.Url,
                    Company: job.Company,
                    CompanyDisplayName: job.CompanyDisplayName,
                    LocationName: job.LocationName,
                    LocationDisplayName: job.LocationDisplayName,
                    IsRemote: job.IsRemote,
                    IsHidden: job.IsHidden,
                    SalaryMin: job.SalaryMin,
                    SalaryMax: job.SalaryMax,
                    SalaryCurrency: job.SalaryCurrency,
                    ContractType: job.ContractType,
                    ContractTime: job.ContractTime,
                    IsFullTime: job.IsFullTime,
                    IsPartTime: job.IsPartTime,
                    IsPermanent: job.IsPermanent,
                    IsContract: job.IsContract,
                    SourceName: job.SourceName,
                    PostedAtUtc: job.PostedAtUtc,
                    IsSaved: false,
                    IsUserHidden: false))
                .ToListAsync(cancellationToken);
        }

        return new Paged<JobSearchResultResponse>(
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalCount: totalCount,
            Items: items);
    }

    public async Task<Paged<JobImportRunResponse>> GetRecentImportRunsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var pageIndex = Math.Max(request.PageIndex, 0);
        var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 50);
        var query = dbContext.JobRefreshRuns
            .AsNoTracking()
            .OrderByDescending(run => run.StartedAtUtc)
            .ThenByDescending(run => run.Id);

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(run => new JobImportRunResponse(
                Id: run.Id,
                ProviderName: run.ProviderName,
                Status: run.Status.ToString(),
                JsonFilter: run.RequestFiltersJson,
                PagesRequested: run.PagesRequested,
                PagesCompleted: run.PagesCompleted,
                RecordsReceived: run.RecordsReceived,
                RecordsInserted: run.RecordsInserted,
                RecordsHidden: run.RecordsHidden,
                RecordsFailed: run.RecordsFailed,
                StartedAtUtc: run.StartedAtUtc,
                CompletedAtUtc: run.CompletedAtUtc,
                FailureSummary: string.IsNullOrWhiteSpace(run.FailureMessage) ? null : run.FailureMessage))
            .ToListAsync(cancellationToken);

        return new Paged<JobImportRunResponse>(
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalCount: totalCount,
            Items: items);
    }

    public async Task<ExportJobsResponse> ExportAsync(ExportJobsRequest request, CancellationToken cancellationToken = default)
    {
        IQueryable<JobPosting> query;

        if (request.JobIds is { Count: > 0 })
        {
            var ids = request.JobIds.Distinct().ToArray();
            query = dbContext.Jobs.AsNoTracking().Where(job => ids.Contains(job.Id));
        }
        else
        {
            query = ApplyFilters(
                dbContext.Jobs.AsNoTracking(),
                new GetJobsPageRequest(
                    PageIndex: 0,
                    PageSize: int.MaxValue,
                    Keyword: request.Keyword,
                    Company: request.Company,
                    Postcode: request.Postcode,
                    Location: request.Location,
                    SourceName: request.SourceName,
                    CategoryTag: request.CategoryTag,
                    IsHidden: request.IsHidden));
        }

        var jobs = await query
            .OrderByDescending(job => job.PostedAtUtc)
            .ThenByDescending(job => job.Id)
            .Select(JobResponseMappers.ToExportRequest())
            .ToListAsync(cancellationToken);

        return new ExportJobsResponse(ExportedAtUtc: DateTime.UtcNow, Count: jobs.Count, Jobs: jobs);
    }

    private static IQueryable<JobPosting> ApplyFilters(IQueryable<JobPosting> query, GetJobsPageRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(job =>
                job.Title.Contains(keyword) ||
                job.Description.Contains(keyword) ||
                job.Summary.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(request.Company))
        {
            var company = request.Company.Trim();
            query = query.Where(job =>
                job.Company.Contains(company) ||
                (job.CompanyDisplayName != null && job.CompanyDisplayName.Contains(company)) ||
                (job.CompanyCanonicalName != null && job.CompanyCanonicalName.Contains(company)));
        }

        if (!string.IsNullOrWhiteSpace(request.Postcode))
        {
            var postcode = request.Postcode.Trim().ToUpperInvariant();
            query = query.Where(job => job.Postcode.Contains(postcode));
        }

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            var location = request.Location.Trim();
            query = query.Where(job =>
                job.LocationName.Contains(location) ||
                (job.LocationDisplayName != null && job.LocationDisplayName.Contains(location)));
        }

        if (!string.IsNullOrWhiteSpace(request.SourceName))
        {
            var sourceName = request.SourceName.Trim();
            query = query.Where(job => job.SourceName.Contains(sourceName));
        }

        if (!string.IsNullOrWhiteSpace(request.CategoryTag))
        {
            var categoryTag = request.CategoryTag.Trim();
            query = query.Where(job => job.CategoryTag != null && job.CategoryTag.Contains(categoryTag));
        }

        if (request.IsHidden.HasValue)
        {
            query = query.Where(job => job.IsHidden == request.IsHidden.Value);
        }

        return query;
    }
}
