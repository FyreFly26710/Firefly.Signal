using Firefly.Signal.Ai.Domain;

namespace Firefly.Signal.Ai.Api.Contracts.Requests;

public sealed record AiChatRequest
{
    public required AiProvider Provider { get; init; }

    /// <summary>
    /// Model identifier, e.g. "gpt-4o" or "deepseek-chat".
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// ID of an existing <c>AiMessage</c> (type SystemPrompt) to use as context.
    /// </summary>
    public long? SystemPromptMessageId { get; init; }

    /// <summary>
    /// Previous conversation turns to build chat history, in chronological order.
    /// </summary>
    public IReadOnlyList<AiChatHistoryItem> History { get; init; } = [];

    /// <summary>
    /// The current user message.
    /// </summary>
    public string? UserPrompt { get; init; }
}
