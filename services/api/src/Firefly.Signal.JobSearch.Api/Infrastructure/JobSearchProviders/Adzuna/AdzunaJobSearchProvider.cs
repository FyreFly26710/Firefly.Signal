using System.Text.Json;
using System.Text.Json.Serialization;
using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Microsoft.Extensions.Options;

namespace Firefly.Signal.JobSearch.Infrastructure.JobSearchProviders.Adzuna;

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
            .AddFrom(providerRequest)
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
        private static readonly Dictionary<string, string> ParameterNames = typeof(AdzunaJobSearchRequest)
            .GetProperties()
            .Where(property => property.GetCustomAttributes(typeof(JsonIgnoreAttribute), inherit: false).Length == 0)
            .ToDictionary(
                property => property.Name,
                property => property.GetCustomAttributes(typeof(JsonPropertyNameAttribute), inherit: false)
                    .Cast<JsonPropertyNameAttribute>()
                    .SingleOrDefault()?.Name ?? property.Name,
                StringComparer.Ordinal);

        private readonly List<string> values = [];

        public QueryStringBuilder Add(string key, string value)
        {
            values.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
            return this;
        }

        public QueryStringBuilder AddFrom(AdzunaJobSearchRequest request)
        {
            foreach (var property in typeof(AdzunaJobSearchRequest).GetProperties())
            {
                if (!ParameterNames.TryGetValue(property.Name, out var parameterName))
                {
                    continue;
                }

                if (property.Name is nameof(AdzunaJobSearchRequest.Location1) or nameof(AdzunaJobSearchRequest.Location2))
                {
                    continue;
                }

                var value = property.GetValue(request);
                if (value is null)
                {
                    continue;
                }

                switch (value)
                {
                    case string text when !string.IsNullOrWhiteSpace(text):
                        Add(parameterName, text);
                        break;
                    case bool flag when property.Name == nameof(AdzunaJobSearchRequest.TitleOnly):
                        if (flag)
                        {
                            Add(parameterName, "1");
                        }

                        break;
                    case bool flag:
                        Add(parameterName, flag ? "1" : "0");
                        break;
                    case int number:
                        Add(parameterName, number.ToString());
                        break;
                    case decimal amount:
                        Add(parameterName, amount.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        break;
                }
            }

            return this;
        }

        public override string ToString() => string.Join("&", values);
    }
}
