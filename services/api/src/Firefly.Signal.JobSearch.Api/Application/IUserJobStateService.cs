namespace Firefly.Signal.JobSearch.Application;

public interface IUserJobStateService
{
    /// <summary>Returns null if the job does not exist.</summary>
    Task<UserJobStateResponse?> SaveJobAsync(long jobId, long userAccountId, CancellationToken cancellationToken = default);

    /// <summary>Returns null if the job does not exist.</summary>
    Task<UserJobStateResponse?> UnsaveJobAsync(long jobId, long userAccountId, CancellationToken cancellationToken = default);

    /// <summary>Returns null if the job does not exist.</summary>
    Task<UserJobStateResponse?> HideJobForUserAsync(long jobId, long userAccountId, CancellationToken cancellationToken = default);

    /// <summary>Returns null if the job does not exist.</summary>
    Task<UserJobStateResponse?> UnhideJobForUserAsync(long jobId, long userAccountId, CancellationToken cancellationToken = default);
}
