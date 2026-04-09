using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Infrastructure.External;

public interface IPublicJobSourceClient
{
    Task<PublicJobSearchResult> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default);
}
