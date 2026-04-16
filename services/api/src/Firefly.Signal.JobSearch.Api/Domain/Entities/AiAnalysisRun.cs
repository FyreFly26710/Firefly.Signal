using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

/// <summary>
/// Tracks one admin-triggered AI analysis request over a set of jobs for a target user.
/// </summary>
public sealed class AiAnalysisRun : AuditableEntity
{
    private AiAnalysisRun()
    {
    }

    public long RequestedByUserAccountId { get; private set; }
    /// <summary>
    /// User whose stored profile and documents are used as the AI comparison context.
    /// </summary>
    public long TargetUserAccountId { get; private set; }
    public AiAnalysisRunMode Mode { get; private set; }
    public AiAnalysisRunStatus Status { get; private set; }
    public int JobCount { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? FailureSummary { get; private set; }

    public static AiAnalysisRun Start(
        long requestedByUserAccountId,
        long targetUserAccountId,
        AiAnalysisRunMode mode,
        int jobCount)
    {
        return new AiAnalysisRun
        {
            RequestedByUserAccountId = requestedByUserAccountId,
            TargetUserAccountId = targetUserAccountId,
            Mode = mode,
            Status = AiAnalysisRunStatus.Running,
            JobCount = jobCount,
            StartedAtUtc = DateTime.UtcNow
        };
    }

    public void Complete(bool partial = false)
    {
        Status = partial ? AiAnalysisRunStatus.PartiallyCompleted : AiAnalysisRunStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        FailureSummary = null;
        Touch();
    }

    public void Fail(string? failureSummary)
    {
        Status = AiAnalysisRunStatus.Failed;
        CompletedAtUtc = DateTime.UtcNow;
        FailureSummary = Normalize(failureSummary);
        Touch();
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
