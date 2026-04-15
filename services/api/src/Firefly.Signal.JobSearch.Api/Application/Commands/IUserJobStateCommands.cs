using Firefly.Signal.JobSearch.Contracts.Responses;

namespace Firefly.Signal.JobSearch.Application.Commands;

public interface IUserJobStateCommands
{
    Task<UserJobStateResponse?> SaveJobAsync(long jobId, long userAccountId, CancellationToken cancellationToken = default);
    Task<UserJobStateResponse?> UnsaveJobAsync(long jobId, long userAccountId, CancellationToken cancellationToken = default);
    Task<UserJobStateResponse?> HideJobForUserAsync(long jobId, long userAccountId, CancellationToken cancellationToken = default);
    Task<UserJobStateResponse?> UnhideJobForUserAsync(long jobId, long userAccountId, CancellationToken cancellationToken = default);
}
