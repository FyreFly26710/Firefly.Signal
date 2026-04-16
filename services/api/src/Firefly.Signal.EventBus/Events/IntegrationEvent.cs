using System.Text.Json.Serialization;
using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.EventBus;

public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = SnowflakeId.GenerateId();
        CreationDateUtc = DateTime.UtcNow;
    }

    [JsonInclude]
    public long Id { get; init; }

    [JsonInclude]
    public DateTime CreationDateUtc { get; init; }
}
