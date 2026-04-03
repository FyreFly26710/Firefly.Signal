namespace Firefly.Signal.IntegrationEventLogEF;

public sealed class IntegrationEventLogEntry
{
    public Guid EventId { get; set; }
    public string EventTypeName { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }
    public string Content { get; set; } = string.Empty;
}
