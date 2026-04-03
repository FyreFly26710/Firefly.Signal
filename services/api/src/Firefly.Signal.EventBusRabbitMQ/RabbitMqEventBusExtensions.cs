using Firefly.Signal.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Firefly.Signal.EventBusRabbitMQ;

public static class RabbitMqEventBusExtensions
{
    private const string EventBusSectionName = "EventBus";

    public static IEventBusBuilder AddRabbitMqEventBus(
        this IHostApplicationBuilder builder,
        string subscriptionClientName)
    {
        builder.Services.Configure<RabbitMqEventBusOptions>(builder.Configuration.GetSection("RabbitMq"));
        builder.Services.Configure<EventBusOptions>(builder.Configuration.GetSection(EventBusSectionName));
        builder.Services.PostConfigure<EventBusOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.SubscriptionClientName))
            {
                options.SubscriptionClientName = subscriptionClientName;
            }
        });
        builder.Services.AddOptions<EventBusSubscriptionInfo>();
        builder.Services.AddSingleton<RabbitMqEventBus>();
        builder.Services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<RabbitMqEventBus>());
        builder.Services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<RabbitMqEventBus>());

        return new DefaultEventBusBuilder(builder.Services);
    }

    private sealed class DefaultEventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services => services;
    }
}
