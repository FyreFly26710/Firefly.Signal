using System.Text.Json.Serialization;

namespace Firefly.Signal.JobSearch.Infrastructure.JobSearchProviders.Adzuna;

public sealed class AdzunaJobSearchResponse
{
    [JsonPropertyName("count")]
    public long? Count { get; init; }

    [JsonPropertyName("results")]
    public IReadOnlyList<AdzunaJobSearchResultResponse> Results { get; init; } = [];
}

public sealed class AdzunaJobSearchResultResponse
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

public sealed class AdzunaCompanyResponse
{
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; init; }
}

public sealed class AdzunaLocationResponse
{
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; init; }
}
