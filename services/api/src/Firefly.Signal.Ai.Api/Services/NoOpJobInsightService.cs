namespace Firefly.Signal.Ai.Api.Services;

public sealed class NoOpJobInsightService : IJobInsightService
{
    public Task<string> SummarizeJobAsync(long jobId, CancellationToken cancellationToken = default)
        => Task.FromResult($"AI enrichment is not connected yet. Placeholder summary for job {jobId}.");
}
