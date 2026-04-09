using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Infrastructure.External;

public sealed record PublicJobSearchResult(
    long TotalCount,
    IReadOnlyList<JobPosting> Jobs);
