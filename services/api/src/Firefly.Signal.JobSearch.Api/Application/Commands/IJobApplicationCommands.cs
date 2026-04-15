using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Application.Commands;

public interface IJobApplicationCommands
{
    Task<JobApplicationResponse?> ApplyJobAsync(long jobId, long userAccountId, string? note, CancellationToken cancellationToken = default);
    Task<JobApplicationResponse?> AdvanceApplicationStatusAsync(long jobId, long userAccountId, JobApplicationStatus newStatus, CancellationToken cancellationToken = default);
    Task<JobApplicationResponse?> UpdateApplicationNoteAsync(long jobId, long userAccountId, string? note, CancellationToken cancellationToken = default);
}
