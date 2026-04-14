using Microsoft.AspNetCore.Http.HttpResults;

namespace Firefly.Signal.JobSearch.Api.Apis;

public static class JobSearchStatusApi
{
    public static IEndpointRouteBuilder MapJobSearchStatusApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", GetServiceStatusAsync);
        return endpoints;
    }

    private static IResult GetServiceStatusAsync()
        => TypedResults.Ok(new
        {
            service = "job-search",
            message = "Firefly Signal job search API is running."
        });
}
