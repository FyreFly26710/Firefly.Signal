using Firefly.Signal.Gateway.Api.Options;
using Firefly.Signal.Gateway.Api.Services;

namespace Firefly.Signal.Gateway.Api.Extensions;

internal static class ApplicationServiceExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddCors(options =>
        {
            options.AddPolicy(GatewayCorsPolicy.Frontend, policy =>
            {
                var configuredOrigins = builder.Configuration
                    .GetSection(GatewayCorsOptions.SectionName)
                    .Get<string[]>();

                var allowedOrigins = configuredOrigins is { Length: > 0 }
                    ? configuredOrigins
                    : builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing")
                        ? LocalCorsOrigins.All
                        : [];

                if (allowedOrigins.Length == 0)
                {
                    return;
                }

                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.Configure<DownstreamOptions>(builder.Configuration.GetSection(DownstreamOptions.SectionName));
        services.AddHttpClient<GatewayDemoClient>();
        services.AddHttpClient<GatewayProxyClient>();
    }
}
