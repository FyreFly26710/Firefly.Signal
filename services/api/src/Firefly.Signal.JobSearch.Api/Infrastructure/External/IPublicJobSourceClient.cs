using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Infrastructure.External;

public interface IPublicJobSourceClient
{
    Task<IReadOnlyList<JobPosting>> SearchAsync(string postcode, string keyword, CancellationToken cancellationToken = default);
}
