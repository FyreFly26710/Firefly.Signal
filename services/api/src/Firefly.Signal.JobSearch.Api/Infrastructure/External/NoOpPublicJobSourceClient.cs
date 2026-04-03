using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Infrastructure.External;

public sealed class NoOpPublicJobSourceClient : IPublicJobSourceClient
{
    public Task<IReadOnlyList<JobPosting>> SearchAsync(string postcode, string keyword, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<JobPosting>>([]);
}
