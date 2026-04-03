using System.Linq.Expressions;
using Firefly.Signal.EventBus;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Firefly.Signal.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public sealed class DbJobSearchService(
    JobSearchDbContext dbContext,
    IPublicJobSourceClient publicJobSourceClient,
    IEventBus eventBus) : IJobSearchService
{
    public async Task<IReadOnlyList<JobCard>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .OrderByDescending(x => x.PostedAtUtc)
            .Select(ToCard())
            .ToListAsync(cancellationToken);
    }

    public async Task<JobCard?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .Where(x => x.Id == id)
            .Select(ToCard())
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<SearchJobsResponse> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedPostcode = request.Postcode.Trim().ToUpperInvariant();
        var normalizedKeyword = request.Keyword.Trim();

        var query = dbContext.Jobs.AsQueryable();
        query = query.Where(x => x.Postcode.Contains(normalizedPostcode) || x.LocationName.Contains(normalizedPostcode));
        query = query.Where(x =>
            x.Title.Contains(normalizedKeyword) ||
            x.Company.Contains(normalizedKeyword) ||
            x.Summary.Contains(normalizedKeyword));

        var totalCount = await query.CountAsync(cancellationToken);
        var jobs = await query
            .OrderByDescending(x => x.PostedAtUtc)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .Select(ToCard())
            .ToListAsync(cancellationToken);

        await publicJobSourceClient.SearchAsync(normalizedPostcode, normalizedKeyword, cancellationToken);
        await eventBus.PublishAsync(new JobSearchRequestedIntegrationEvent(
            SearchId: SnowflakeId.GenerateId(),
            Postcode: normalizedPostcode,
            Keyword: normalizedKeyword,
            ResultCount: totalCount), cancellationToken);

        return new SearchJobsResponse(request.Postcode, request.Keyword, request.PageIndex, request.PageSize, totalCount, jobs);
    }

    private static Expression<Func<JobPosting, JobCard>> ToCard()
        => job => new JobCard(
            job.Id,
            job.Title,
            job.Company,
            job.LocationName,
            job.Summary,
            job.Url,
            job.SourceName,
            job.IsRemote,
            job.PostedAtUtc);
}
