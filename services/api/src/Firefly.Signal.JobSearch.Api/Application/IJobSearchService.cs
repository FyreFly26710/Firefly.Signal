using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Application;

public interface IJobSearchService
{
    Task<SearchJobsResponse> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobCard>> ListAsync(CancellationToken cancellationToken = default);
    Task<JobCard?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
