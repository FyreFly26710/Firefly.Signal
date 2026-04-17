using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

/// <summary>
/// Records a single stage reached in a job application, with the date it occurred.
/// One row is written per status transition; the full set of entries forms the application timeline.
/// </summary>
public sealed class JobApplicationStatusEntry : AuditableEntity
{
    private JobApplicationStatusEntry()
    {
    }

    public long JobApplicationId { get; private set; }
    public JobApplicationStatus Status { get; private set; }
    public int? RoundNumber { get; private set; }
    public DateTime StatusAtUtc { get; private set; }

    public static JobApplicationStatusEntry Create(
        long jobApplicationId,
        JobApplicationStatus status,
        int? roundNumber = null,
        DateTime? statusAtUtc = null)
    {
        return new JobApplicationStatusEntry
        {
            JobApplicationId = jobApplicationId,
            Status = status,
            RoundNumber = roundNumber,
            StatusAtUtc = statusAtUtc ?? DateTime.UtcNow
        };
    }
}
