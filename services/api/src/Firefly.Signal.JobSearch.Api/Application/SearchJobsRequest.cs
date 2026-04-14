namespace Firefly.Signal.JobSearch.Application;

public sealed record SearchJobsRequest(
    string Location,
    string? Keyword,
    int PageIndex = 0,
    int PageSize = 20,
    JobSearchProviderKind Provider = JobSearchProviderKind.Adzuna,
    string? ExcludedKeyword = null,
    int? DistanceKilometers = null,
    string? Category = null,
    decimal? SalaryMin = null,
    decimal? SalaryMax = null,
    bool? FullTime = null,
    bool? PartTime = null,
    bool? Permanent = null,
    bool? Contract = null,
    string? SortBy = null,
    int? MaxDaysOld = null,
    string? Company = null,
    bool TitleOnly = false,
    string? Location0 = null,
    string? Location1 = null,
    string? Location2 = null);
