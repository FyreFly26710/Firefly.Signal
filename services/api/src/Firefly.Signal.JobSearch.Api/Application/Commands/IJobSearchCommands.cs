using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Contracts.Responses;

namespace Firefly.Signal.JobSearch.Application.Commands;

public interface IJobSearchCommands
{
    Task<JobDetailsResponse> CreateAsync(CreateJobRequest request, CancellationToken cancellationToken = default);
    Task<JobDetailsResponse?> UpdateAsync(long id, UpdateJobRequest request, CancellationToken cancellationToken = default);
    Task<ImportJobsResponse> ImportFromProviderAsync(ImportJobsFromProviderRequest request, CancellationToken cancellationToken = default);
    Task<ImportJobsResponse> ImportFromJsonAsync(Stream jsonStream, string fileName, CancellationToken cancellationToken = default);
    Task<HideJobsResponse> HideAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
    Task<DeleteJobsResponse> DeleteAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
}
