using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

// TODO(real-ai-flow): Remove this temporary demo entity once the real JobSearch AI request/result
// persistence model is implemented.
public sealed class UserJobAiChatDemoRun : AuditableEntity
{
    private UserJobAiChatDemoRun()
    {
    }

    public long UserAccountId { get; private set; }
    public long JobPostingId { get; private set; }
    public string CorrelationId { get; private set; } = string.Empty;
    public string Provider { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public string Prompt { get; private set; } = string.Empty;
    public UserJobAiChatDemoStatus Status { get; private set; }
    public long? AiResponseId { get; private set; }
    public string? AiResponseContent { get; private set; }
    public DateTime RequestedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    public static UserJobAiChatDemoRun Start(
        long userAccountId,
        long jobPostingId,
        string correlationId,
        string provider,
        string model,
        string prompt)
    {
        return new UserJobAiChatDemoRun
        {
            UserAccountId = userAccountId,
            JobPostingId = jobPostingId,
            CorrelationId = NormalizeRequired(correlationId, nameof(correlationId), 128),
            Provider = NormalizeRequired(provider, nameof(provider), 64),
            Model = NormalizeRequired(model, nameof(model), 128),
            Prompt = NormalizeRequired(prompt, nameof(prompt), 12000),
            Status = UserJobAiChatDemoStatus.Pending,
            RequestedAtUtc = DateTime.UtcNow
        };
    }

    public void Complete(long responseId, string responseContent)
    {
        if (responseId <= 0)
            throw new ArgumentOutOfRangeException(nameof(responseId));

        AiResponseId = responseId;
        AiResponseContent = NormalizeRequired(responseContent, nameof(responseContent), 12000);
        Status = UserJobAiChatDemoStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void Fail(string? responseContent)
    {
        AiResponseContent = NormalizeOptional(responseContent, 12000);
        Status = UserJobAiChatDemoStatus.Failed;
        CompletedAtUtc = DateTime.UtcNow;
        Touch();
    }

    private static string NormalizeRequired(string value, string paramName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new ArgumentOutOfRangeException(paramName, $"Value exceeds max length {maxLength}.");

        return normalized;
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();
        return normalized.Length > maxLength ? normalized[..maxLength] : normalized;
    }
}
