using Firefly.Signal.EventBus;

namespace Firefly.Signal.Ai.Api.IntegrationEvents;

/// <summary>
/// Published by another service (e.g. job-search-api) to request an AI completion.
/// The AI API handles this event, calls the provider, and publishes
/// <see cref="AiChatCompletedIntegrationEvent"/> back to the caller.
/// </summary>
public sealed record AiChatRequestedIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Opaque token the caller uses to match the async reply to the original request.
    /// </summary>
    public required string CorrelationId { get; init; }

    /// <summary>
    /// Logical name of the service that dispatched this event (e.g. "job-search-api").
    /// Used to route the reply back correctly.
    /// </summary>
    public required string CallerService { get; init; }

    /// <summary>
    /// Integration event type name the caller expects as the reply.
    /// </summary>
    public required string ReplyEventType { get; init; }

    /// <summary>
    /// Model identifier, e.g. "gpt-4o" or "deepseek-chat".
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// Full task context encoded as a system prompt.
    /// </summary>
    public required string SystemPrompt { get; init; }
}
