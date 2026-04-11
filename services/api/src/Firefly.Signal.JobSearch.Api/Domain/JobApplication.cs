using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

/// <summary>
/// Captures the persisted user-specific application record for a job.
/// </summary>
public sealed class JobApplication : AuditableEntity, IAggregateRoot
{
    private JobApplication()
    {
    }

    public long UserAccountId { get; private set; }
    public long JobPostingId { get; private set; }
    public DateTime AppliedAtUtc { get; private set; }
    public JobApplicationStatus Status { get; private set; }
    /// <summary>
    /// User document selected as the CV actually submitted for this application.
    /// </summary>
    public long? SubmittedCvDocumentId { get; private set; }
    /// <summary>
    /// User document selected as the cover letter actually submitted for this application.
    /// </summary>
    public long? SubmittedCoverLetterDocumentId { get; private set; }
    public DateTime? RejectionAtUtc { get; private set; }
    public string? RejectionReason { get; private set; }

    public static JobApplication Create(
        long userAccountId,
        long jobPostingId,
        long? submittedCvDocumentId,
        long? submittedCoverLetterDocumentId,
        DateTime? appliedAtUtc = null)
    {
        return new JobApplication
        {
            UserAccountId = userAccountId,
            JobPostingId = jobPostingId,
            AppliedAtUtc = appliedAtUtc ?? DateTime.UtcNow,
            Status = JobApplicationStatus.Applied,
            SubmittedCvDocumentId = submittedCvDocumentId,
            SubmittedCoverLetterDocumentId = submittedCoverLetterDocumentId
        };
    }

    public void UpdateSubmittedDocuments(long? submittedCvDocumentId, long? submittedCoverLetterDocumentId)
    {
        SubmittedCvDocumentId = submittedCvDocumentId;
        SubmittedCoverLetterDocumentId = submittedCoverLetterDocumentId;
        Touch();
    }

    public void MarkRejected(string? rejectionReason, DateTime? rejectedAtUtc = null)
    {
        Status = JobApplicationStatus.Rejected;
        RejectionReason = Normalize(rejectionReason);
        RejectionAtUtc = rejectedAtUtc ?? DateTime.UtcNow;
        Touch();
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
