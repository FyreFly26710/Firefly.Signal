using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Application;

public sealed record SearchJobsResponse(
    string Postcode,
    string Keyword,
    int PageIndex,
    int PageSize,
    long TotalCount,
    IReadOnlyList<JobCard> Jobs);
