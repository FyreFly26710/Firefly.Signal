using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Firefly.Signal.EventBus;

public sealed class EventBusSubscriptionInfo
{
    public Dictionary<string, Type> EventTypes { get; } = [];

    public JsonSerializerOptions JsonSerializerOptions { get; } = new(DefaultJsonSerializerOptions);

    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        TypeInfoResolver = JsonSerializer.IsReflectionEnabledByDefault
            ? new DefaultJsonTypeInfoResolver()
            : JsonTypeInfoResolver.Combine()
    };
}
