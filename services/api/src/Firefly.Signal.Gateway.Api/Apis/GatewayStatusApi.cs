using Firefly.Signal.Gateway.Api.Models;
using Firefly.Signal.Gateway.Api.Options;
using Firefly.Signal.Gateway.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Firefly.Signal.Gateway.Api.Apis;

public static class GatewayStatusApi
{
    public static RouteGroupBuilder MapGatewayStatusApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", GetServiceStatusAsync);

        var group = endpoints.MapGroup("/api/demo");
        group.MapGet("/topology", GetTopologyAsync);
        group.MapGet("/status", GetStatusAsync);

        return group;
    }

    private static IResult GetServiceStatusAsync()
        => TypedResults.Ok(new
        {
            service = "gateway",
            message = "Firefly Signal backend gateway demo is running."
        });

    private static Ok<DownstreamOptions> GetTopologyAsync(IConfiguration configuration)
    {
        var options = configuration
            .GetSection(DownstreamOptions.SectionName)
            .Get<DownstreamOptions>() ?? new DownstreamOptions();

        return TypedResults.Ok(options);
    }

    private static async Task<Ok<GatewayStatusResponse>> GetStatusAsync(
        GatewayDemoClient client,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await client.GetStatusAsync(cancellationToken));
}
