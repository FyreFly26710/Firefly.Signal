using Firefly.Signal.JobSearch.Application;

namespace Firefly.Signal.JobSearch.Contracts.Requests;

public sealed record ImportJobsFromProviderRequest(
    int PageIndex = 1,
    int PageSize = 50,
    string Where = "london",
    string? Keyword = null,
    int DistanceKilometers = 5,
    int MaxDaysOld = 30,
    string Category = "it-jobs",
    JobSearchProviderKind Provider = JobSearchProviderKind.Adzuna,
    string? ExcludedKeyword = null,
    decimal? SalaryMin = null,
    decimal? SalaryMax = null);
