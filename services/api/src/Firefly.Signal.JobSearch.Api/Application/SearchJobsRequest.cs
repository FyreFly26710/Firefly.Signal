namespace Firefly.Signal.JobSearch.Application;

public sealed record SearchJobsRequest(
    string Postcode,
    string Keyword,
    int PageIndex = 0,
    int PageSize = 20,
    JobSearchProviderKind Provider = JobSearchProviderKind.Adzuna);
