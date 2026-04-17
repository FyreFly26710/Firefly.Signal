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
