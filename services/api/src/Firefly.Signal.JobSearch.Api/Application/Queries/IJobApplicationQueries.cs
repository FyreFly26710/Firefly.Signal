using Firefly.Signal.JobSearch.Contracts.Responses;

namespace Firefly.Signal.JobSearch.Application.Queries;

public interface IJobApplicationQueries
{
    Task<IReadOnlyList<AppliedJobSummaryResponse>> GetAppliedJobsAsync(long userAccountId, CancellationToken cancellationToken = default);
    Task<AppliedJobDetailResponse?> GetAppliedJobDetailAsync(long applicationId, long userAccountId, CancellationToken cancellationToken = default);
}
