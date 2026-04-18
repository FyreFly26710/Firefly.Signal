namespace Firefly.Signal.EventBus.Events.Ai;

public sealed record AiChatRequestedIntegrationEvent : IntegrationEvent
{
    public required string CorrelationId { get; init; }
    public required string CallerService { get; init; }
    public required string ReplyEventType { get; init; }
    public required string Provider { get; init; }
    public required string Model { get; init; }
    public long? SystemPromptMessageId { get; init; }
    public string? UserPrompt { get; init; }
}
