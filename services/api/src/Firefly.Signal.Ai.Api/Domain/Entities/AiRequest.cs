using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.Ai.Domain;

/// <summary>
/// Tracks a single AI chat request from submission through to completion or failure.
/// Covers both direct HTTP calls and fire-and-forget MQ event-driven calls from other services.
/// Message content (prompts and response) is stored exclusively in AiMessage.
/// </summary>
public sealed class AiRequest : AuditableEntity
{
    public long? SystemPromptMessageId { get; private set; }
    public long? UserPromptMessageId { get; private set; }

    /// <summary>
    /// Whether the request arrived via HTTP or a MQ integration event.
    /// </summary>
    public AiRequestSource Source { get; private set; }

    /// <summary>
    /// Lifecycle status of this request.
    /// </summary>
    public AiRequestStatus Status { get; private set; }

    /// <summary>
    /// Provider that was used (or will be used) to fulfil this request.
    /// Set when the request transitions to Processing.
    /// </summary>
    public AiProvider Provider { get; private set; }

    /// <summary>
    /// model supplied by the caller (e.g. "gpt-4o").
    /// </summary>
    public string Model { get; private set; } = string.Empty;

    // ── MQ routing ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Opaque identifier supplied by an MQ caller so it can correlate the async response.
    /// Null for HTTP-sourced requests.
    /// </summary>
    public string? CorrelationId { get; private set; }

    /// <summary>
    /// Logical name of the service that dispatched the MQ event (e.g. "job-search-api").
    /// Used to route the reply event back to the correct service.
    /// Null for HTTP-sourced requests.
    /// </summary>
    public string? CallerService { get; private set; }

    /// <summary>
    /// Integration event type name to publish when this request completes.
    /// The consuming service uses this to identify the inbound reply event.
    /// Null for HTTP-sourced requests.
    /// </summary>
    public string? ReplyEventType { get; private set; }

    // ── Timestamps ─────────────────────────────────────────────────────────────

    public DateTime QueuedAtUtc { get; private set; }
    public DateTime? ProcessingStartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    // ── Failure ────────────────────────────────────────────────────────────────

    public string? FailureSummary { get; private set; }

    // ── Computed ───────────────────────────────────────────────────────────────

    public bool IsTerminal => Status is AiRequestStatus.Completed or AiRequestStatus.Failed;

    // ── Navigation ─────────────────────────────────────────────────────────────

    public AiResponse? Response { get; private set; }
    public AiMessage? SystemPromptMessage { get; private set; }
    public AiMessage? UserPromptMessage { get; private set; }

    // ── Factory ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates an HTTP-sourced request. Starts in Queued status.
    /// </summary>
    public static AiRequest QueueHttp(string model, string? systemPromptMessage, string? userPromptMessage)
    {
        return new AiRequest
        {
            Source = AiRequestSource.Http,
            Status = AiRequestStatus.Queued,
            Model = Normalize(model),
            SystemPromptMessage = string.IsNullOrWhiteSpace(systemPromptMessage) ? null : new AiMessage(AiMessageType.SystemPrompt, systemPromptMessage),
            UserPromptMessage = string.IsNullOrWhiteSpace(userPromptMessage) ? null : new AiMessage(AiMessageType.UserPrompt, userPromptMessage),
            QueuedAtUtc = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an MQ-sourced request. Starts in Queued status.
    /// The full task context is encoded in the system prompt — no user prompt message.
    /// </summary>
    public static AiRequest QueueFromEvent(
        string model,
        string systemPromptMessage,
        string correlationId,
        string callerService,
        string replyEventType)
    {
        if (string.IsNullOrWhiteSpace(correlationId))
            throw new ArgumentException("Correlation ID is required for MQ-sourced requests.", nameof(correlationId));

        if (string.IsNullOrWhiteSpace(callerService))
            throw new ArgumentException("Caller service is required for MQ-sourced requests.", nameof(callerService));

        if (string.IsNullOrWhiteSpace(replyEventType))
            throw new ArgumentException("Reply event type is required for MQ-sourced requests.", nameof(replyEventType));

        return new AiRequest
        {
            Source = AiRequestSource.MqEvent,
            Status = AiRequestStatus.Queued,
            Model = Normalize(model),
            SystemPromptMessage = new AiMessage(AiMessageType.SystemPrompt, systemPromptMessage),
            UserPromptMessage = null,
            CorrelationId = correlationId.Trim(),
            CallerService = callerService.Trim(),
            ReplyEventType = replyEventType.Trim(),
            QueuedAtUtc = DateTime.UtcNow
        };
    }

    // ── State transitions ──────────────────────────────────────────────────────

    public void StartProcessing(AiProvider provider)
    {
        EnsureQueued();

        Provider = provider;
        Status = AiRequestStatus.Processing;
        ProcessingStartedAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void Complete(AiResponse response)
    {
        EnsureProcessing();

        Response = response ?? throw new ArgumentNullException(nameof(response));
        Status = AiRequestStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void Fail(string failureSummary)
    {
        if (IsTerminal)
            throw new InvalidOperationException("AI request is already in a terminal state.");

        if (string.IsNullOrWhiteSpace(failureSummary))
            throw new ArgumentException("Failure summary is required.", nameof(failureSummary));

        Status = AiRequestStatus.Failed;
        FailureSummary = failureSummary.Trim();
        CompletedAtUtc = DateTime.UtcNow;
        Touch();
    }

    // ── Guards ─────────────────────────────────────────────────────────────────

    private void EnsureQueued()
    {
        if (Status != AiRequestStatus.Queued)
            throw new InvalidOperationException($"Expected Queued status but was {Status}.");
    }

    private void EnsureProcessing()
    {
        if (Status != AiRequestStatus.Processing)
            throw new InvalidOperationException($"Expected Processing status but was {Status}.");
    }

    private static string Normalize(string value) => value.Trim();
}
