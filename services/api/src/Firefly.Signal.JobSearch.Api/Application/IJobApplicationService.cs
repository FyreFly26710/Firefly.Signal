using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Application;

public interface IJobApplicationService
{
    /// <summary>
    /// Creates or returns the existing application for the user+job pair.
    /// Returns null if the job does not exist.
    /// </summary>
    Task<JobApplicationResponse?> ApplyJobAsync(long jobId, long userAccountId, string? note, CancellationToken cancellationToken = default);

    /// <summary>
    /// Advances the application status. Returns null if no application exists.
    /// Throws InvalidOperationException on an invalid transition.
    /// </summary>
    Task<JobApplicationResponse?> AdvanceApplicationStatusAsync(long jobId, long userAccountId, JobApplicationStatus newStatus, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppliedJobSummaryResponse>> GetAppliedJobsAsync(long userAccountId, CancellationToken cancellationToken = default);
}
