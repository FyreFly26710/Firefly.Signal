using Firefly.Signal.JobSearch.Contracts.Responses;

namespace Firefly.Signal.JobSearch.Application.Queries;

public interface IJobApplicationQueries
{
    Task<IReadOnlyList<AppliedJobSummaryResponse>> GetAppliedJobsAsync(long userAccountId, CancellationToken cancellationToken = default);
}
