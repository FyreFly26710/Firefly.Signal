using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Application;

public interface IJobSearchService
{
    Task<SearchJobsResponse> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default);
}
