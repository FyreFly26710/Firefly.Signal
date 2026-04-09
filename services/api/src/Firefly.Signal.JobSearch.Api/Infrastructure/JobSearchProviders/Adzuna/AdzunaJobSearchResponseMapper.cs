using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.External;

namespace Firefly.Signal.JobSearch.Infrastructure.JobSearchProviders.Adzuna;

public sealed class AdzunaJobSearchResponseMapper
{
    public PublicJobSearchResult Map(AdzunaJobSearchResponse? response, AdzunaJobSearchRequest request)
    {
        if (response is null)
        {
            return new PublicJobSearchResult(0, []);
        }

        var jobs = response.Results
            .Select(job => JobPosting.Create(
                job.Title ?? "Untitled role",
                job.Company?.DisplayName ?? "Unknown company",
                request.Where,
                job.Location?.DisplayName ?? request.Where,
                job.Description ?? string.Empty,
                job.RedirectUrl ?? string.Empty,
                "Adzuna",
                IsRemote(job),
                ParsePostedAtUtc(job.Created)))
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
}
