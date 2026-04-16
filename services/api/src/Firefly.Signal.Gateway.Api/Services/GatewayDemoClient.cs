using Firefly.Signal.Gateway.Api.Models;
using Firefly.Signal.Gateway.Api.Options;

namespace Firefly.Signal.Gateway.Api.Services;

internal sealed class GatewayDemoClient(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<GatewayStatusResponse> GetStatusAsync(CancellationToken cancellationToken)
    {
        var options = configuration
            .GetSection(DownstreamOptions.SectionName)
            .Get<DownstreamOptions>() ?? new DownstreamOptions();

        var identity = await httpClient.GetFromJsonAsync<object>($"{options.IdentityApiBaseUrl}/", cancellationToken);
        var jobs = await httpClient.GetFromJsonAsync<object>($"{options.JobSearchApiBaseUrl}/", cancellationToken);
        var ai = await httpClient.GetFromJsonAsync<object>($"{options.AiApiBaseUrl}/", cancellationToken);

        return new GatewayStatusResponse("gateway", identity, jobs, ai);
    }
}
