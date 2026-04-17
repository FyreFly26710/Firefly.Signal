using System.Text.Json;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;

namespace Firefly.Signal.JobSearch.Infrastructure.JobSearchProviders.Adzuna;

public sealed class AdzunaJobSearchResponseMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public PublicJobSearchResult Map(AdzunaJobSearchResponse? response, AdzunaJobSearchRequest request)
    {
        if (response is null)
        {
            return new PublicJobSearchResult(0, []);
        }

        var jobs = response.Results
            .Select(job => JobPosting.Create(
                jobRefreshRunId: null,
                sourceName: "Adzuna",
                sourceJobId: job.Id ?? string.Empty,
                sourceAdReference: job.AdReference,
                title: job.Title ?? "Untitled role",
                description: job.Description ?? string.Empty,
                summary: job.Description ?? string.Empty,
                url: job.RedirectUrl ?? string.Empty,
                company: job.Company?.DisplayName ?? "Unknown company",
                companyDisplayName: job.Company?.DisplayName,
                companyCanonicalName: job.Company?.CanonicalName,
                postcode: request.Where,
                locationName: job.Location?.DisplayName ?? request.Where,
                locationDisplayName: job.Location?.DisplayName,
                locationAreaJson: SerializeLocationArea(job.Location?.Area),
                latitude: job.Latitude,
                longitude: job.Longitude,
                categoryTag: job.Category?.Tag,
                categoryLabel: job.Category?.Label,
                salaryMin: job.SalaryMin,
                salaryMax: job.SalaryMax,
                salaryCurrency: null,
                salaryIsPredicted: ParseSalaryIsPredicted(job.SalaryIsPredicted),
                contractTime: job.ContractTime,
                contractType: job.ContractType,
                isFullTime: string.Equals(job.ContractTime, "full_time", StringComparison.OrdinalIgnoreCase),
                isPartTime: string.Equals(job.ContractTime, "part_time", StringComparison.OrdinalIgnoreCase),
                isPermanent: string.Equals(job.ContractType, "permanent", StringComparison.OrdinalIgnoreCase),
                isContract: string.Equals(job.ContractType, "contract", StringComparison.OrdinalIgnoreCase),
                isRemote: IsRemote(job),
                postedAtUtc: ParsePostedAtUtc(job.Created),
                importedAtUtc: DateTime.UtcNow,
                lastSeenAtUtc: DateTime.UtcNow,
                isHidden: false,
                rawPayloadJson: JsonSerializer.Serialize(job, JsonOptions)))
            .ToArray();

        return new PublicJobSearchResult(response.Count ?? jobs.LongLength, jobs);
    }

    private static DateTime ParsePostedAtUtc(string? value)
        => DateTimeOffset.TryParse(value, out var parsed)
            ? parsed.UtcDateTime
            : DateTime.UtcNow;

    private static bool IsRemote(AdzunaJobSearchResultResponse job)
        => ContainsRemote(job.Title) || ContainsRemote(job.Description);

    private static bool ContainsRemote(string? value)
        => !string.IsNullOrWhiteSpace(value) &&
           value.Contains("remote", StringComparison.OrdinalIgnoreCase);

    private static bool? ParseSalaryIsPredicted(string? value)
        => value switch
        {
            "1" => true,
            "0" => false,
            _ => null
        };

    private static string? SerializeLocationArea(IReadOnlyList<string>? area)
        => area is { Count: > 0 }
            ? JsonSerializer.Serialize(area, JsonOptions)
            : null;
}
