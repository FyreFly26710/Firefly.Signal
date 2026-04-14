using Firefly.Signal.Ai.Api.Models;
using Firefly.Signal.Ai.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Firefly.Signal.Ai.Api.Apis;

public static class AiApi
{
    public static RouteGroupBuilder MapAiApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/ai");
        group.MapGet("/job-insights/{jobId:long}", GetJobInsightAsync);
        return group;
    }

    private static async Task<Ok<JobInsightResponse>> GetJobInsightAsync(
        long jobId,
        IJobInsightService service,
        CancellationToken cancellationToken)
    {
        var summary = await service.SummarizeJobAsync(jobId, cancellationToken);
        return TypedResults.Ok(new JobInsightResponse(jobId, summary, "noop"));
    }
}
