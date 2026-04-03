using Firefly.Signal.Ai.Api.Services;

namespace Firefly.Signal.Ai.Api.Endpoints;

public static class AiEndpoints
{
    public static IEndpointRouteBuilder MapAiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/ai");
        group.MapGet("/job-insights/{jobId:long}", GetJobInsightAsync);
        return endpoints;
    }

    private static async Task<IResult> GetJobInsightAsync(long jobId, IJobInsightService service, CancellationToken cancellationToken)
    {
        var summary = await service.SummarizeJobAsync(jobId, cancellationToken);
        return Results.Ok(new { jobId, summary, provider = "noop" });
    }
}
