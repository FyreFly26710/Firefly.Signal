using Firefly.Signal.EventBus;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public sealed class DbJobSearchService(
    IEnumerable<IJobSearchProvider> providers,
    IEventBus eventBus) : IJobSearchService
{
    public async Task<SearchJobsResponse> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedPostcode = request.Postcode.Trim().ToUpperInvariant();
        var normalizedKeyword = request.Keyword.Trim();
        var normalizedRequest = request with
        {
            Postcode = normalizedPostcode,
            Keyword = normalizedKeyword
        };
        var provider = providers.SingleOrDefault(x => x.Provider == normalizedRequest.Provider);
        if (provider is null)
        {
            throw new JobSearchProviderException($"The {normalizedRequest.Provider} job search provider is not registered.");
        }

        var providerResult = await provider.SearchAsync(normalizedRequest, cancellationToken);

        await eventBus.PublishAsync(new JobSearchRequestedIntegrationEvent(
            SearchId: SnowflakeId.GenerateId(),
            Postcode: normalizedPostcode,
            Keyword: normalizedKeyword,
            ResultCount: providerResult.TotalCount > int.MaxValue ? int.MaxValue : (int)providerResult.TotalCount), cancellationToken);

        return new SearchJobsResponse(
            request.Postcode,
            request.Keyword,
            request.PageIndex,
            request.PageSize,
            providerResult.TotalCount,
            providerResult.Jobs.Select(ToCard()).ToArray());
    }

    private static Func<JobPosting, JobCard> ToCard()
        => job => new JobCard(
            CreateCardId(job),
            job.Title,
            job.Company,
            job.LocationName,
            job.Summary,
            job.Url,
            job.SourceName,
            job.IsRemote,
            job.PostedAtUtc);

    private static string CreateCardId(JobPosting job)
        => string.IsNullOrWhiteSpace(job.Url)
            ? $"{job.SourceName}:{job.Title}:{job.Company}"
            : job.Url;
}
