namespace Firefly.Signal.Ai.Api.Services;

public interface IJobInsightService
{
    Task<string> SummarizeJobAsync(long jobId, CancellationToken cancellationToken = default);
}
