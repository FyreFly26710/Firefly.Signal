using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

/// <summary>
/// Stores a user-specific lightweight workflow state for a job.
/// IsSaved and IsHidden are independent flags; application stage tracking lives on JobApplication.
/// </summary>
public sealed class UserJobState : AuditableEntity, IAggregateRoot
{
    private UserJobState()
    {
    }

    public long UserAccountId { get; private set; }
    public long JobPostingId { get; private set; }
    public bool IsSaved { get; private set; }
    public bool IsHidden { get; private set; }
    public DateTime LastUpdatedAtUtc { get; private set; }

    public static UserJobState Create(long userAccountId, long jobPostingId)
    {
        return new UserJobState
        {
            UserAccountId = userAccountId,
            JobPostingId = jobPostingId,
            LastUpdatedAtUtc = DateTime.UtcNow
        };
    }

    public void MarkSaved()
    {
        IsSaved = true;
        LastUpdatedAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void Unsave()
    {
        IsSaved = false;
        LastUpdatedAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void Hide()
    {
        IsHidden = true;
        LastUpdatedAtUtc = DateTime.UtcNow;
        Touch();
    }

    public void Unhide()
    {
        IsHidden = false;
        LastUpdatedAtUtc = DateTime.UtcNow;
        Touch();
    }
}
