namespace Firefly.Signal.JobSearch.Application;

public interface IJobSearchService
{
    Task<JobDetailsResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<PagedJobsResponse> GetPageAsync(GetJobsPageRequest request, CancellationToken cancellationToken = default);
    Task<JobDetailsResponse> CreateAsync(CreateJobRequest request, CancellationToken cancellationToken = default);
    Task<JobDetailsResponse?> UpdateAsync(long id, UpdateJobRequest request, CancellationToken cancellationToken = default);
    Task<HideJobsResponse> HideAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
    Task<DeleteJobsResponse> DeleteAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
}
