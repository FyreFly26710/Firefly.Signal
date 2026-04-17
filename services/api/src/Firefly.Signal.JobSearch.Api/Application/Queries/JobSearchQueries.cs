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
        SearchJobsPageRequest request,
        long? userId,
        CancellationToken cancellationToken = default)
    {
        var pageIndex = Math.Max(request.PageIndex, 0);
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

        var query = ApplySearchFilters(dbContext.Jobs.AsQueryable(), request);
        var totalCount = await query.LongCountAsync(cancellationToken);
        var orderedQuery = ApplySearchOrdering(query, request.SortBy, request.IsAsc);

        List<JobSearchResultResponse> items;

        if (userId.HasValue)
        {
            var userStates = dbContext.UserJobStates.Where(state => state.UserAccountId == userId.Value);

            items = await (
                from job in orderedQuery
                join state in userStates on job.Id equals state.JobPostingId into joined
                from state in joined.DefaultIfEmpty()
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
            items = await orderedQuery
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
                JsonFilter: JobSearchQueriesHelpers.NormalizeJsonFilter(run.RequestFiltersJson),
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
            throw new ArgumentException("At least one JobId must be provided for export.", nameof(request.JobIds));
        }

        var jobs = await query
            .OrderByDescending(job => job.PostedAtUtc)
            .ThenByDescending(job => job.Id)
            .Select(JobResponseMappers.ToExportRequest())
            .ToListAsync(cancellationToken);

        return new ExportJobsResponse(ExportedAtUtc: DateTime.UtcNow, Count: jobs.Count, Jobs: jobs);
    }

    private static IQueryable<JobPosting> ApplySearchFilters(IQueryable<JobPosting> query, SearchJobsPageRequest request)
    {
        // Always exclude catalog-hidden jobs on the public search page.
        query = query.Where(job => !job.IsHidden);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(job => job.Title.Contains(keyword));
        }

        // request.Where is intentionally not applied here.
        // It captures the free-text "town or area" input from the search UI
        // but requires geospatial distance search (postcode lookup + radius query)
        // to be implemented first. TODO: wire up once distance-based search is available.

        if (request.SalaryMin.HasValue  && request.SalaryMin.Value > 0)
        {
            query = query.Where(job => job.SalaryMax >= request.SalaryMin.Value);
        }

        if (request.SalaryMax.HasValue && request.SalaryMax.Value > 0)
        {
            query = query.Where(job => job.SalaryMin <= request.SalaryMax.Value);
        }

        var dateCutoff = GetDatePostedCutoff(request.DatePosted);
        if (dateCutoff.HasValue)
        {
            query = query.Where(job => job.PostedAtUtc >= dateCutoff.Value);
        }

        return query;
    }

    private static IOrderedQueryable<JobPosting> ApplySearchOrdering(IQueryable<JobPosting> query, string? sortBy, bool isAsc)
    {
        if (sortBy?.ToLowerInvariant() == "salary")
        {
            return isAsc
                ? query.OrderBy(job => job.SalaryMin ?? 0).ThenByDescending(job => job.PostedAtUtc)
                : query.OrderByDescending(job => job.SalaryMin ?? 0).ThenByDescending(job => job.PostedAtUtc);
        }

        // Default: sort by date
        return isAsc
            ? query.OrderBy(job => job.PostedAtUtc).ThenBy(job => job.Id)
            : query.OrderByDescending(job => job.PostedAtUtc).ThenByDescending(job => job.Id);
    }

    /// <summary>
    /// Returns the UTC cutoff time for the given day window.
    /// A value of N means "posted within the last N days" — cutoff is midnight UTC N days ago.
    /// </summary>
    private static DateTime? GetDatePostedCutoff(int? datePosted)
    {
        if (!datePosted.HasValue || datePosted.Value <= 0) return null;
        return DateTime.UtcNow.Date.AddDays(-datePosted.Value);
    }

}
