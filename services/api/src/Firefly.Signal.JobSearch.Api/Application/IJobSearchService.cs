using Firefly.Signal.SharedKernel.Models;

namespace Firefly.Signal.JobSearch.Application;

public interface IJobSearchService
{
    Task<JobDetailsResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Paged<JobSearchResultResponse>> SearchPageAsync(GetJobsPageRequest request, long? userId, CancellationToken cancellationToken = default);
    Task<ExportJobsResponse> ExportAsync(ExportJobsRequest request, CancellationToken cancellationToken = default);
    Task<JobDetailsResponse> CreateAsync(CreateJobRequest request, CancellationToken cancellationToken = default);
    Task<JobDetailsResponse?> UpdateAsync(long id, UpdateJobRequest request, CancellationToken cancellationToken = default);
    Task<ImportJobsResponse> ImportFromProviderAsync(ImportJobsFromProviderRequest request, CancellationToken cancellationToken = default);
    Task<ImportJobsResponse> ImportFromJsonAsync(Stream jsonStream, string fileName, CancellationToken cancellationToken = default);
    Task<HideJobsResponse> HideAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
    Task<DeleteJobsResponse> DeleteAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
}
