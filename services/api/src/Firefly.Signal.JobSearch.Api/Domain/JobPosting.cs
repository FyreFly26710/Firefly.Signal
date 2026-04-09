using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

public sealed class JobPosting : AuditableEntity, IAggregateRoot
{
    private JobPosting()
    {
    }

    // Links the imported job back to the admin-triggered refresh run that fetched it.
    public long? JobRefreshRunId { get; private set; }
    // Provider name such as Adzuna.
    public string SourceName { get; private set; } = string.Empty;
    // Provider-owned identifier for the listing.
    public string SourceJobId { get; private set; } = string.Empty;
    // Provider reference used to re-fetch or inspect the listing later.
    public string? SourceAdReference { get; private set; }
    // Human-readable job title from the provider payload.
    public string Title { get; private set; } = string.Empty;
    // Longer provider description retained for local search and admin review.
    public string Description { get; private set; } = string.Empty;
    // Shorter summary copy used by the current app contract.
    public string Summary { get; private set; } = string.Empty;
    // Redirect URL that should be used to send the user to the original listing.
    public string Url { get; private set; } = string.Empty;
    // App-friendly company name used by the current UI contract.
    public string Company { get; private set; } = string.Empty;
    // Company name exactly as provided by the source payload.
    public string? CompanyDisplayName { get; private set; }
    // Normalized company name when the provider supplies one.
    public string? CompanyCanonicalName { get; private set; }
    // Simple postcode/search-area value retained for current local filtering.
    public string Postcode { get; private set; } = string.Empty;
    // App-friendly location name used by the current UI contract.
    public string LocationName { get; private set; } = string.Empty;
    // Full provider location display text.
    public string? LocationDisplayName { get; private set; }
    // Denormalized provider location hierarchy stored as JSON text.
    public string? LocationAreaJson { get; private set; }
    // Optional latitude returned by the provider.
    public decimal? Latitude { get; private set; }
    // Optional longitude returned by the provider.
    public decimal? Longitude { get; private set; }
    // Provider category key used for filtering or reporting.
    public string? CategoryTag { get; private set; }
    // Human-readable provider category label.
    public string? CategoryLabel { get; private set; }
    // Lower bound of the advertised salary range.
    public decimal? SalaryMin { get; private set; }
    // Upper bound of the advertised salary range.
    public decimal? SalaryMax { get; private set; }
    // Currency code or label if later supplied or inferred.
    public string? SalaryCurrency { get; private set; }
    // Whether the provider marked the salary as predicted.
    public bool? SalaryIsPredicted { get; private set; }
    // Raw contract-time value from the provider.
    public string? ContractTime { get; private set; }
    // Raw contract-type value from the provider.
    public string? ContractType { get; private set; }
    // Convenience flag for local filtering.
    public bool IsFullTime { get; private set; }
    // Convenience flag for local filtering.
    public bool IsPartTime { get; private set; }
    // Convenience flag for local filtering.
    public bool IsPermanent { get; private set; }
    // Convenience flag for local filtering.
    public bool IsContract { get; private set; }
    // Derived remote hint based on provider content.
    public bool IsRemote { get; private set; }
    // Provider-created or provider-published timestamp for the listing.
    public DateTime PostedAtUtc { get; private set; }
    // When Firefly imported this row into the local catalog.
    public DateTime ImportedAtUtc { get; private set; }
    // Most recent refresh time that returned this listing.
    public DateTime LastSeenAtUtc { get; private set; }
    // Manual moderation flag that excludes the row from normal user search.
    public bool IsHidden { get; private set; }
    // Raw provider payload preserved for audit/debug and future remapping.
    public string RawPayloadJson { get; private set; } = "{}";

    public static JobPosting Create(
        string title,
        string company,
        string postcode,
        string locationName,
        string summary,
        string url,
        string sourceName,
        bool isRemote,
        DateTime postedAtUtc)
        => Create(
            jobRefreshRunId: null,
            sourceName: sourceName,
            sourceJobId: string.Empty,
            sourceAdReference: null,
            title: title,
            description: summary,
            summary: summary,
            url: url,
            company: company,
            companyDisplayName: company,
            companyCanonicalName: company,
            postcode: postcode,
            locationName: locationName,
            locationDisplayName: locationName,
            locationAreaJson: null,
            latitude: null,
            longitude: null,
            categoryTag: null,
            categoryLabel: null,
            salaryMin: null,
            salaryMax: null,
            salaryCurrency: null,
            salaryIsPredicted: null,
            contractTime: null,
            contractType: null,
            isFullTime: false,
            isPartTime: false,
            isPermanent: false,
            isContract: false,
            isRemote: isRemote,
            postedAtUtc: postedAtUtc,
            importedAtUtc: DateTime.UtcNow,
            lastSeenAtUtc: DateTime.UtcNow,
            isHidden: false,
            rawPayloadJson: "{}");

    public static JobPosting Create(
        long? jobRefreshRunId,
        string sourceName,
        string sourceJobId,
        string? sourceAdReference,
        string title,
        string description,
        string summary,
        string url,
        string company,
        string? companyDisplayName,
        string? companyCanonicalName,
        string postcode,
        string locationName,
        string? locationDisplayName,
        string? locationAreaJson,
        decimal? latitude,
        decimal? longitude,
        string? categoryTag,
        string? categoryLabel,
        decimal? salaryMin,
        decimal? salaryMax,
        string? salaryCurrency,
        bool? salaryIsPredicted,
        string? contractTime,
        string? contractType,
        bool isFullTime,
        bool isPartTime,
        bool isPermanent,
        bool isContract,
        bool isRemote,
        DateTime postedAtUtc,
        DateTime importedAtUtc,
        DateTime lastSeenAtUtc,
        bool isHidden,
        string rawPayloadJson)
    {
        return new JobPosting
        {
            JobRefreshRunId = jobRefreshRunId,
            SourceName = sourceName.Trim(),
            SourceJobId = sourceJobId.Trim(),
            SourceAdReference = string.IsNullOrWhiteSpace(sourceAdReference) ? null : sourceAdReference.Trim(),
            Title = title.Trim(),
            Description = description.Trim(),
            Company = company.Trim(),
            CompanyDisplayName = string.IsNullOrWhiteSpace(companyDisplayName) ? null : companyDisplayName.Trim(),
            CompanyCanonicalName = string.IsNullOrWhiteSpace(companyCanonicalName) ? null : companyCanonicalName.Trim(),
            Postcode = postcode.Trim().ToUpperInvariant(),
            LocationName = locationName.Trim(),
            LocationDisplayName = string.IsNullOrWhiteSpace(locationDisplayName) ? null : locationDisplayName.Trim(),
            LocationAreaJson = string.IsNullOrWhiteSpace(locationAreaJson) ? null : locationAreaJson.Trim(),
            Summary = summary.Trim(),
            Url = url.Trim(),
            Latitude = latitude,
            Longitude = longitude,
            CategoryTag = string.IsNullOrWhiteSpace(categoryTag) ? null : categoryTag.Trim(),
            CategoryLabel = string.IsNullOrWhiteSpace(categoryLabel) ? null : categoryLabel.Trim(),
            SalaryMin = salaryMin,
            SalaryMax = salaryMax,
            SalaryCurrency = string.IsNullOrWhiteSpace(salaryCurrency) ? null : salaryCurrency.Trim(),
            SalaryIsPredicted = salaryIsPredicted,
            ContractTime = string.IsNullOrWhiteSpace(contractTime) ? null : contractTime.Trim(),
            ContractType = string.IsNullOrWhiteSpace(contractType) ? null : contractType.Trim(),
            IsFullTime = isFullTime,
            IsPartTime = isPartTime,
            IsPermanent = isPermanent,
            IsContract = isContract,
            IsRemote = isRemote,
            PostedAtUtc = postedAtUtc,
            ImportedAtUtc = importedAtUtc,
            LastSeenAtUtc = lastSeenAtUtc,
            IsHidden = isHidden,
            RawPayloadJson = string.IsNullOrWhiteSpace(rawPayloadJson) ? "{}" : rawPayloadJson.Trim()
        };
    }

    public void Hide()
    {
        IsHidden = true;
        Touch();
    }

    public void Show()
    {
        IsHidden = false;
        Touch();
    }

    public void Update(
        long? jobRefreshRunId,
        string sourceName,
        string sourceJobId,
        string? sourceAdReference,
        string title,
        string description,
        string summary,
        string url,
        string company,
        string? companyDisplayName,
        string? companyCanonicalName,
        string postcode,
        string locationName,
        string? locationDisplayName,
        string? locationAreaJson,
        decimal? latitude,
        decimal? longitude,
        string? categoryTag,
        string? categoryLabel,
        decimal? salaryMin,
        decimal? salaryMax,
        string? salaryCurrency,
        bool? salaryIsPredicted,
        string? contractTime,
        string? contractType,
        bool isFullTime,
        bool isPartTime,
        bool isPermanent,
        bool isContract,
        bool isRemote,
        DateTime postedAtUtc,
        DateTime importedAtUtc,
        DateTime lastSeenAtUtc,
        bool isHidden,
        string rawPayloadJson)
    {
        JobRefreshRunId = jobRefreshRunId;
        SourceName = sourceName.Trim();
        SourceJobId = sourceJobId.Trim();
        SourceAdReference = string.IsNullOrWhiteSpace(sourceAdReference) ? null : sourceAdReference.Trim();
        Title = title.Trim();
        Description = description.Trim();
        Summary = summary.Trim();
        Url = url.Trim();
        Company = company.Trim();
        CompanyDisplayName = string.IsNullOrWhiteSpace(companyDisplayName) ? null : companyDisplayName.Trim();
        CompanyCanonicalName = string.IsNullOrWhiteSpace(companyCanonicalName) ? null : companyCanonicalName.Trim();
        Postcode = postcode.Trim().ToUpperInvariant();
        LocationName = locationName.Trim();
        LocationDisplayName = string.IsNullOrWhiteSpace(locationDisplayName) ? null : locationDisplayName.Trim();
        LocationAreaJson = string.IsNullOrWhiteSpace(locationAreaJson) ? null : locationAreaJson.Trim();
        Latitude = latitude;
        Longitude = longitude;
        CategoryTag = string.IsNullOrWhiteSpace(categoryTag) ? null : categoryTag.Trim();
        CategoryLabel = string.IsNullOrWhiteSpace(categoryLabel) ? null : categoryLabel.Trim();
        SalaryMin = salaryMin;
        SalaryMax = salaryMax;
        SalaryCurrency = string.IsNullOrWhiteSpace(salaryCurrency) ? null : salaryCurrency.Trim();
        SalaryIsPredicted = salaryIsPredicted;
        ContractTime = string.IsNullOrWhiteSpace(contractTime) ? null : contractTime.Trim();
        ContractType = string.IsNullOrWhiteSpace(contractType) ? null : contractType.Trim();
        IsFullTime = isFullTime;
        IsPartTime = isPartTime;
        IsPermanent = isPermanent;
        IsContract = isContract;
        IsRemote = isRemote;
        PostedAtUtc = postedAtUtc;
        ImportedAtUtc = importedAtUtc;
        LastSeenAtUtc = lastSeenAtUtc;
        IsHidden = isHidden;
        RawPayloadJson = string.IsNullOrWhiteSpace(rawPayloadJson) ? "{}" : rawPayloadJson.Trim();
        Touch();
    }
}
