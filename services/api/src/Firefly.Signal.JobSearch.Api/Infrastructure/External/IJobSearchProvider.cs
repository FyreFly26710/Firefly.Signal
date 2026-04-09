using Firefly.Signal.JobSearch.Application;

namespace Firefly.Signal.JobSearch.Infrastructure.External;

public interface IJobSearchProvider
{
    JobSearchProviderKind Provider { get; }

    Task<PublicJobSearchResult> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default);
}
