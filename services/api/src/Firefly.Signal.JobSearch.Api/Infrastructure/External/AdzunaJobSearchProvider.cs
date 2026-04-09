using System.Text.Json;
using Firefly.Signal.JobSearch.Application;
using Microsoft.Extensions.Options;

namespace Firefly.Signal.JobSearch.Infrastructure.External;

public sealed class AdzunaJobSearchProvider(
    HttpClient httpClient,
    IOptions<AdzunaOptions> options,
    AdzunaJobSearchRequestMapper requestMapper,
    AdzunaJobSearchResponseMapper responseMapper,
    ILogger<AdzunaJobSearchProvider> logger) : IJobSearchProvider
{
    public JobSearchProviderKind Provider => JobSearchProviderKind.Adzuna;

    public async Task<PublicJobSearchResult> SearchAsync(SearchJobsRequest request, CancellationToken cancellationToken = default)
    {
        var providerOptions = options.Value;

        if (string.IsNullOrWhiteSpace(providerOptions.AppId) || string.IsNullOrWhiteSpace(providerOptions.AppKey))
        {
            throw new JobSearchProviderException("Adzuna credentials are not configured for the job search service.");
        }

        var providerRequest = requestMapper.Map(request);
        var baseUri = new Uri(providerOptions.BaseUrl.TrimEnd('/'));
        var query = new QueryStringBuilder()
            .Add("app_id", providerOptions.AppId)
            .Add("app_key", providerOptions.AppKey)
            .Add("results_per_page", providerRequest.ResultsPerPage.ToString())
            .Add("what", providerRequest.What)
            .Add("where", providerRequest.Where)
            .Add("content-type", "application/json");

        var requestUri = new Uri(baseUri, $"/v1/api/jobs/{providerOptions.CountryCode}/search/{providerRequest.PageNumber}?{query}");
        using var response = await httpClient.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Adzuna job search failed with status code {StatusCode}.", (int)response.StatusCode);
            throw new JobSearchProviderException("The configured job search provider could not return results right now.");
        }

        var payloadBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var payload = JsonSerializer.Deserialize<AdzunaJobSearchResponse>(payloadBytes);
        return responseMapper.Map(payload, providerRequest);
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
