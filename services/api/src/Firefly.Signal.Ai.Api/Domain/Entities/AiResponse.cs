using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.Ai.Domain;

/// <summary>
/// Metadata record for a completed AI response.
/// Actual response content is stored in the linked AiMessage with type AgentResponse.
/// </summary>
public sealed class AiResponse : AuditableEntity
{
    private AiResponse()
    {
    }

    public long AiRequestId { get; private set; }
    public long AiResponseMessageId { get; private set; }

    /// <summary>
    /// Number of tokens consumed by the prompt. Null if the provider did not return usage.
    /// </summary>
    public int? PromptTokens { get; private set; }

    /// <summary>
    /// Number of tokens in the completion. Null if the provider did not return usage.
    /// </summary>
    public int? CompletionTokens { get; private set; }

    public DateTime GeneratedAtUtc { get; private set; }

    // ── Computed ───────────────────────────────────────────────────────────────

    public int? TotalTokens => PromptTokens.HasValue && CompletionTokens.HasValue
        ? PromptTokens + CompletionTokens
        : null;


    // ── Navigation ─────────────────────────────────────────────────────────────

    public AiMessage Message { get; private set; } = null!;

    // ── Factory ────────────────────────────────────────────────────────────────

    public static AiResponse Create(
        long aiRequestId,
        string aiResponseMessage,
        int? promptTokens = null,
        int? completionTokens = null)
    {
        return new AiResponse
        {
            AiRequestId = aiRequestId, 
            Message = new AiMessage(AiMessageType.AgentResponse, aiResponseMessage),
            PromptTokens = promptTokens,
            CompletionTokens = completionTokens,
            GeneratedAtUtc = DateTime.UtcNow
        };
    }
}
