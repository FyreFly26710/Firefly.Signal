using Firefly.Signal.SharedKernel.Models;

namespace Firefly.Signal.JobSearch.Application;

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
    string Where = "london", //location or postcode
    string? Keyword = null, 
    int DistanceKilometers = 5,
    int MaxDaysOld = 30, // days old of a job post
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
