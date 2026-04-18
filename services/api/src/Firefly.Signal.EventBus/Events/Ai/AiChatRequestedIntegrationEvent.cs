namespace Firefly.Signal.EventBus.Events.Ai;

public sealed record AiChatRequestedIntegrationEvent : IntegrationEvent
{
    public required string CorrelationId { get; init; }
    public required string CallerService { get; init; }
    public required string ReplyEventType { get; init; }
    public required string Provider { get; init; }
    public required string Model { get; init; }
    // TODO(real-ai-flow): Make this required again once JobSearch can provide the real shared
    // system prompt message id instead of using the current mock prompt-only path.
    public long? SystemPromptMessageId { get; init; }
    public string? UserPrompt { get; init; }
}
