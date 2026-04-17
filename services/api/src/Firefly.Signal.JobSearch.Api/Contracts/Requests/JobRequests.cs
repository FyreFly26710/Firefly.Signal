using Firefly.Signal.JobSearch.Application;

namespace Firefly.Signal.JobSearch.Contracts.Requests;

/// <summary>
/// Request model for the public job search page endpoint.
/// All filter fields are optional — an empty request returns all visible jobs.
/// </summary>
public sealed record SearchJobsPageRequest(
    /// <summary>Keyword matched against job title only.</summary>
    string? Keyword = null,

    /// <summary>
    /// Town, city or free-text area name entered in the "Where" field.
    /// Not currently applied to queries — geospatial distance search requires
    /// a postcode-lookup and radius query implementation.
    /// TODO: wire up once distance-based search is available.
    /// </summary>
    string? Where = null,

    /// <summary>Minimum annual salary filter (inclusive).</summary>
    decimal? SalaryMin = null,

    /// <summary>Maximum annual salary filter (inclusive).</summary>
    decimal? SalaryMax = null,

    /// <summary>
    /// Recency window in days. A value of N returns jobs posted within the last N days
    /// (cutoff = today − N days at midnight UTC). Null or 0 means no date filter.
    /// </summary>
    int? DatePosted = null,

    /// <summary>
    /// Field to sort by. Accepted values: "date" (default), "salary" (sorts by SalaryMin).
    /// </summary>
    string? SortBy = null,

    /// <summary>When true results are returned ascending; default is descending.</summary>
    bool IsAsc = false,

    int PageIndex = 0,
    int PageSize = 20);

public sealed record GetJobsPageRequest(
    int PageIndex = 0,
    int PageSize = 20,
    string? Keyword = null,
    string? Company = null,
    string? Postcode = null,
    string? Location = null,
    string? SourceName = null,
    string? CategoryTag = null,
    bool? IsHidden = false);

public sealed record ExportJobsRequest(
    IReadOnlyList<long>? JobIds = null,
    string? Keyword = null,
    string? Company = null,
    string? Postcode = null,
    string? Location = null,
    string? SourceName = null,
    string? CategoryTag = null,
    bool? IsHidden = null);

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

public sealed record CreateJobRequest(
    long? JobRefreshRunId,
    string SourceName,
    string SourceJobId,
    string? SourceAdReference,
    string Title,
    string Description,
    string Summary,
    string Url,
    string Company,
    string? CompanyDisplayName,
    string? CompanyCanonicalName,
    string Postcode,
    string LocationName,
    string? LocationDisplayName,
    string? LocationAreaJson,
    decimal? Latitude,
    decimal? Longitude,
    string? CategoryTag,
    string? CategoryLabel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? SalaryCurrency,
    bool? SalaryIsPredicted,
    string? ContractTime,
    string? ContractType,
    bool IsFullTime,
    bool IsPartTime,
    bool IsPermanent,
    bool IsContract,
    bool IsRemote,
    DateTime PostedAtUtc,
    DateTime ImportedAtUtc,
    DateTime LastSeenAtUtc,
    bool IsHidden,
    string RawPayloadJson);

public sealed record UpdateJobRequest(
    long? JobRefreshRunId,
    string SourceName,
    string SourceJobId,
    string? SourceAdReference,
    string Title,
    string Description,
    string Summary,
    string Url,
    string Company,
    string? CompanyDisplayName,
    string? CompanyCanonicalName,
    string Postcode,
    string LocationName,
    string? LocationDisplayName,
    string? LocationAreaJson,
    decimal? Latitude,
    decimal? Longitude,
    string? CategoryTag,
    string? CategoryLabel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? SalaryCurrency,
    bool? SalaryIsPredicted,
    string? ContractTime,
    string? ContractType,
    bool IsFullTime,
    bool IsPartTime,
    bool IsPermanent,
    bool IsContract,
    bool IsRemote,
    DateTime PostedAtUtc,
    DateTime ImportedAtUtc,
    DateTime LastSeenAtUtc,
    bool IsHidden,
    string RawPayloadJson);
