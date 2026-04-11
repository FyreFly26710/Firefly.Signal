using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

/// <summary>
/// Stores a user-specific workflow state for a job without mutating the shared job record.
/// </summary>
public sealed class UserJobState : AuditableEntity, IAggregateRoot
{
    private UserJobState()
    {
    }

    public long UserAccountId { get; private set; }
    public long JobPostingId { get; private set; }
    public UserJobWorkflowState State { get; private set; }
    public DateTime? SavedAtUtc { get; private set; }
    public DateTime? AppliedAtUtc { get; private set; }
    public DateTime? RejectedAtUtc { get; private set; }
    /// <summary>
    /// Explicit workflow timestamp kept separate from the shared audit fields for easier product-level state tracking.
    /// </summary>
    public DateTime LastUpdatedAtUtc { get; private set; }

    public static UserJobState Create(long userAccountId, long jobPostingId, UserJobWorkflowState state)
    {
        var entity = new UserJobState
        {
            UserAccountId = userAccountId,
            JobPostingId = jobPostingId
        };

        entity.SetState(state, DateTime.UtcNow);
        return entity;
    }

    public void MarkSaved() => SetState(UserJobWorkflowState.Saved, DateTime.UtcNow);

    public void MarkApplied() => SetState(UserJobWorkflowState.Applied, DateTime.UtcNow);

    public void MarkRejected() => SetState(UserJobWorkflowState.Rejected, DateTime.UtcNow);

    private void SetState(UserJobWorkflowState state, DateTime timestampUtc)
    {
        State = state;

        if (state == UserJobWorkflowState.Saved)
        {
            SavedAtUtc ??= timestampUtc;
            AppliedAtUtc = null;
            RejectedAtUtc = null;
        }
        else if (state == UserJobWorkflowState.Applied)
        {
            SavedAtUtc ??= timestampUtc;
            AppliedAtUtc ??= timestampUtc;
            RejectedAtUtc = null;
        }
        else if (state == UserJobWorkflowState.Rejected)
        {
            SavedAtUtc ??= timestampUtc;
            RejectedAtUtc ??= timestampUtc;
        }

        LastUpdatedAtUtc = timestampUtc;
        Touch();
    }
}
