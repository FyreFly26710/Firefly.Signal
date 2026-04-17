using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.Ai.Domain;

/// <summary>
/// Immutable append-only log of every message exchanged in an AI request.
/// Records system prompts, user prompts, and raw agent response JSON exactly as sent/received.
/// Rows are never updated or deleted — only inserted.
/// </summary>
public sealed class AiMessage : Entity
{
    private AiMessage()
    {
    }

    public AiMessageType Type { get; private set; }

    /// <summary>
    /// Raw content. Plain text for SystemPrompt and UserPrompt.
    /// Raw JSON payload for AgentResponse (the full provider response object).
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public AiMessage(AiMessageType type, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        Type = type;
        Content = content.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }
}
