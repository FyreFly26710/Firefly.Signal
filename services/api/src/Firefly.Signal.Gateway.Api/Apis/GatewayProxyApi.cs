using Firefly.Signal.Gateway.Api.Models;
using Firefly.Signal.Gateway.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Firefly.Signal.Gateway.Api.Apis;

public static class GatewayProxyApi
{
    public static IEndpointRouteBuilder MapGatewayProxyApi(this IEndpointRouteBuilder endpoints)
    {
        MapProxyRoute(endpoints, "/api/auth/{**catchAll}", DownstreamService.Identity);
        MapProxyRoute(endpoints, "/api/auth", DownstreamService.Identity);
        MapProxyRoute(endpoints, "/api/users/{**catchAll}", DownstreamService.Identity);
        MapProxyRoute(endpoints, "/api/users", DownstreamService.Identity);
        MapProxyRoute(endpoints, "/api/job-search/{**catchAll}", DownstreamService.JobSearch);
        MapProxyRoute(endpoints, "/api/job-search", DownstreamService.JobSearch);
        MapProxyRoute(endpoints, "/api/ai/{**catchAll}", DownstreamService.Ai);
        MapProxyRoute(endpoints, "/api/ai", DownstreamService.Ai);

        return endpoints;
    }

    private static void MapProxyRoute(IEndpointRouteBuilder endpoints, string pattern, DownstreamService service)
    {
        endpoints.MapMethods(pattern, [HttpMethods.Options], NoContentHandler);
        endpoints.MapMethods(pattern, GatewayProxyClient.AllowedMethods, ForwardAsync);

        IResult NoContentHandler() => TypedResults.NoContent();

        Task ForwardAsync(HttpContext context, GatewayProxyClient client)
            => client.ForwardAsync(context, service);
    }
}
