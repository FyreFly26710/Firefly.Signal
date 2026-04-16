using Firefly.Signal.JobSearch.Contracts.Requests;

namespace Firefly.Signal.JobSearch.Contracts.Responses;

public sealed record JobDetailsResponse(
    long Id,
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

public sealed record JobSearchResultResponse(
    long Id,
    string SourceJobId,
    string Title,
    string Summary,
    string Url,
    string Company,
    string? CompanyDisplayName,
    string LocationName,
    string? LocationDisplayName,
    bool IsRemote,
    bool IsHidden,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? SalaryCurrency,
    string? ContractType,
    string? ContractTime,
    bool IsFullTime,
    bool IsPartTime,
    bool IsPermanent,
    bool IsContract,
    string SourceName,
    DateTime PostedAtUtc,
    bool IsSaved,
    bool IsUserHidden);

public sealed record HideJobsResponse(
    int HiddenCount,
    IReadOnlyList<long> HiddenIds,
    IReadOnlyList<long> MissingIds);

public sealed record DeleteJobsResponse(
    int DeletedCount,
    IReadOnlyList<long> DeletedIds,
    IReadOnlyList<long> MissingIds,
    IReadOnlyList<long> NotHiddenIds);

public sealed record ImportJobsResponse(
    long JobRefreshRunId,
    string Source,
    int ImportedCount,
    int FailedCount);

public sealed record ExportJobsResponse(
    DateTime ExportedAtUtc,
    int Count,
    IReadOnlyList<CreateJobRequest> Jobs);
