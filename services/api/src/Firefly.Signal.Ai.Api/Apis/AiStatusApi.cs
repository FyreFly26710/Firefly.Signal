using Microsoft.AspNetCore.Http.HttpResults;

namespace Firefly.Signal.Ai.Api.Apis;

public static class AiStatusApi
{
    public static IEndpointRouteBuilder MapAiStatusApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", GetServiceStatusAsync);
        return endpoints;
    }

    private static IResult GetServiceStatusAsync()
        => TypedResults.Ok(new
        {
            service = "ai",
            message = "Firefly Signal AI API is running."
        });
}
