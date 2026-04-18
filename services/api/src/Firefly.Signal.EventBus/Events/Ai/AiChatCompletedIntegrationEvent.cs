namespace Firefly.Signal.EventBus.Events.Ai;

public sealed record AiChatCompletedIntegrationEvent : IntegrationEvent
{
    public required string CorrelationId { get; init; }
    public required string ResponseContent { get; init; }
    public required long ResponseId { get; init; }
    public int? PromptTokens { get; init; }
    public int? CompletionTokens { get; init; }
}
