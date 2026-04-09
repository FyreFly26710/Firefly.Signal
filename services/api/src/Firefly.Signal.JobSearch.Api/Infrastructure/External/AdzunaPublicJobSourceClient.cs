using System.Text.Json;
using System.Text.Json.Serialization;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Microsoft.Extensions.Options;

namespace Firefly.Signal.JobSearch.Infrastructure.External;

public sealed class AdzunaPublicJobSourceClient(
    HttpClient httpClient,
    IOptions<AdzunaOptions> options,
    ILogger<AdzunaPublicJobSourceClient> logger) : IPublicJobSourceClient
{
    public async Task<PublicJobSearchResult> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default)
    {
        var providerOptions = options.Value;

        if (string.IsNullOrWhiteSpace(providerOptions.AppId) || string.IsNullOrWhiteSpace(providerOptions.AppKey))
        {
            throw new JobSearchProviderException("Adzuna credentials are not configured for the job search service.");
        }

        var pageNumber = request.PageIndex + 1;
        var baseUri = new Uri(providerOptions.BaseUrl.TrimEnd('/'));
        var query = new QueryStringBuilder()
            .Add("app_id", providerOptions.AppId)
            .Add("app_key", providerOptions.AppKey)
            .Add("results_per_page", request.PageSize.ToString())
            .Add("what", request.Keyword)
            .Add("where", request.Postcode)
            .Add("content-type", "application/json");

        var requestUri = new Uri(baseUri, $"/v1/api/jobs/{providerOptions.CountryCode}/search/{pageNumber}?{query}");

        using var response = await httpClient.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Adzuna job search failed with status code {StatusCode}.", (int)response.StatusCode);
            throw new JobSearchProviderException("The configured job search provider could not return results right now.");
        }

        var payloadBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var payload = JsonSerializer.Deserialize<AdzunaSearchResponse>(payloadBytes);
        if (payload is null)
        {
            return new PublicJobSearchResult(0, []);
        }

        var jobs = payload.Results
            .Select(job => JobPosting.Create(
                job.Title ?? "Untitled role",
                job.Company?.DisplayName ?? "Unknown company",
                request.Postcode,
                job.Location?.DisplayName ?? request.Postcode,
                job.Description ?? string.Empty,
                job.RedirectUrl ?? string.Empty,
                "Adzuna",
                IsRemote(job),
                ParsePostedAtUtc(job.Created)))
            .ToArray();

        return new PublicJobSearchResult(payload.Count ?? jobs.LongLength, jobs);
    }

    private static DateTime ParsePostedAtUtc(string? value)
        => DateTimeOffset.TryParse(value, out var parsed)
            ? parsed.UtcDateTime
            : DateTime.UtcNow;

    private static bool IsRemote(AdzunaJobResponse job)
        => ContainsRemote(job.Title) || ContainsRemote(job.Description);

    private static bool ContainsRemote(string? value)
        => !string.IsNullOrWhiteSpace(value) &&
           value.Contains("remote", StringComparison.OrdinalIgnoreCase);

    private sealed class AdzunaSearchResponse
    {
        [JsonPropertyName("count")]
        public long? Count { get; init; }

        [JsonPropertyName("results")]
        public IReadOnlyList<AdzunaJobResponse> Results { get; init; } = [];
    }

    private sealed class AdzunaJobResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; init; }

        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("redirect_url")]
        public string? RedirectUrl { get; init; }

        [JsonPropertyName("created")]
        public string? Created { get; init; }

        [JsonPropertyName("company")]
        public AdzunaCompanyResponse? Company { get; init; }

        [JsonPropertyName("location")]
        public AdzunaLocationResponse? Location { get; init; }
    }

    private sealed class AdzunaCompanyResponse
    {
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; init; }
    }

    private sealed class AdzunaLocationResponse
    {
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; init; }
    }

    private sealed class QueryStringBuilder
    {
        private readonly List<string> values = [];

        public QueryStringBuilder Add(string key, string value)
        {
            values.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
            return this;
        }

        public override string ToString() => string.Join("&", values);
    }
}
