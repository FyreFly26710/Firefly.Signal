using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Signal.EventBus;

public static class EventBusBuilderExtensions
{
    public static IEventBusBuilder ConfigureJsonOptions(this IEventBusBuilder builder, Action<JsonSerializerOptions> configure)
    {
        builder.Services.Configure<EventBusSubscriptionInfo>(options => configure(options.JsonSerializerOptions));
        return builder;
    }

    public static IEventBusBuilder AddSubscription<TEvent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
        this IEventBusBuilder builder)
        where TEvent : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<TEvent>
    {
        builder.Services.AddKeyedTransient<IIntegrationEventHandler, THandler>(typeof(TEvent));
        builder.Services.Configure<EventBusSubscriptionInfo>(options =>
        {
            options.EventTypes[typeof(TEvent).Name] = typeof(TEvent);
        });

        return builder;
    }
}
