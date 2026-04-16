using Firefly.Signal.Gateway.Api.Models;

namespace Firefly.Signal.Gateway.Api.Options;

internal sealed class DownstreamOptions
{
    public const string SectionName = "Downstream";

    public string IdentityApiBaseUrl { get; init; } = "http://localhost:5081";
    public string JobSearchApiBaseUrl { get; init; } = "http://localhost:5082";
    public string AiApiBaseUrl { get; init; } = "http://localhost:5083";

    public string GetBaseUrl(DownstreamService service)
        => service switch
        {
            DownstreamService.Identity => IdentityApiBaseUrl,
            DownstreamService.JobSearch => JobSearchApiBaseUrl,
            DownstreamService.Ai => AiApiBaseUrl,
            _ => throw new ArgumentOutOfRangeException(nameof(service), service, "Unknown downstream service.")
        };
}
