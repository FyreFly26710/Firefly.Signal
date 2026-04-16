using Firefly.Signal.JobSearch.Contracts.Requests;

namespace Firefly.Signal.JobSearch.Application.Commands;

internal sealed record ImportedJobsFile(
    DateTime? ExportedAtUtc,
    int? Count,
    IReadOnlyList<CreateJobRequest> Jobs);
