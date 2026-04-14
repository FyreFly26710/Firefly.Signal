using Microsoft.AspNetCore.Http.HttpResults;

namespace Firefly.Signal.Identity.Api.Apis;

public static class IdentityStatusApi
{
    public static IEndpointRouteBuilder MapIdentityStatusApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", GetServiceStatusAsync);
        return endpoints;
    }

    private static IResult GetServiceStatusAsync()
        => TypedResults.Ok(new
        {
            service = "identity",
            message = "Firefly Signal identity API is running."
        });
}
