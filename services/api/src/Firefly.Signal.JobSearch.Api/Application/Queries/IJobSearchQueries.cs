using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.SharedKernel.Models;

namespace Firefly.Signal.JobSearch.Application.Queries;

public interface IJobSearchQueries
{
    Task<JobDetailsResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Paged<JobSearchResultResponse>> SearchPageAsync(GetJobsPageRequest request, long? userId, CancellationToken cancellationToken = default);
    Task<Paged<JobImportRunResponse>> GetRecentImportRunsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<ExportJobsResponse> ExportAsync(ExportJobsRequest request, CancellationToken cancellationToken = default);
}
