using Firefly.Signal.Ai.Api.Consumers;
using Firefly.Signal.Ai.Api.Services;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBusRabbitMQ;

namespace Firefly.Signal.Ai.Api.Extensions;

internal static class ApplicationServiceExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddSingleton<IJobInsightService, NoOpJobInsightService>();

        builder.AddRabbitMqEventBus("ai-api")
            .AddSubscription<JobSearchRequestedIntegrationEvent, JobSearchRequestedIntegrationEventHandler>();
    }
}
