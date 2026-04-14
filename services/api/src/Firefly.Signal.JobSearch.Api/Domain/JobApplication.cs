using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

/// <summary>
/// Anchor record for a user's application to a job.
/// Status history is tracked in JobApplicationStatusEntry.
/// Documents are linked via ApplicationDocumentLink.
/// </summary>
public sealed class JobApplication : AuditableEntity, IAggregateRoot
{
    private JobApplication()
    {
    }

    public long UserAccountId { get; private set; }
    public long JobPostingId { get; private set; }
    public string? Note { get; private set; }

    public static JobApplication Create(long userAccountId, long jobPostingId, string? note = null)
    {
        return new JobApplication
        {
            UserAccountId = userAccountId,
            JobPostingId = jobPostingId,
            Note = Normalize(note)
        };
    }

    public void UpdateNote(string? note)
    {
        Note = Normalize(note);
        Touch();
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
