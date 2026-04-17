namespace Firefly.Signal.JobSearch.Contracts.Requests;

public sealed record ExportJobsRequest(
    IReadOnlyList<long>? JobIds = null);
