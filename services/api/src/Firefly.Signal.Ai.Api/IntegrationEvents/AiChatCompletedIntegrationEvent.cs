using Firefly.Signal.EventBus;

namespace Firefly.Signal.Ai.Api.IntegrationEvents;

/// <summary>
/// Published by the AI API once it has a response from the provider.
/// The caller service uses <see cref="CorrelationId"/> to match this reply
/// to the original <see cref="AiChatRequestedIntegrationEvent"/>.
/// </summary>
public sealed record AiChatCompletedIntegrationEvent : IntegrationEvent
{
    public required string CorrelationId { get; init; }
    public required string ResponseContent { get; init; }
    public int? PromptTokens { get; init; }
    public int? CompletionTokens { get; init; }
}
