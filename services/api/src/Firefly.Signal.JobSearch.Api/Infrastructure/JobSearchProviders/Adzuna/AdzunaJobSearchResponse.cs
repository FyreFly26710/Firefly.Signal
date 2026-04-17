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

    [JsonPropertyName("adref")]
    public string? AdReference { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; init; }

    [JsonPropertyName("created")]
    public string? Created { get; init; }

    [JsonPropertyName("latitude")]
    public decimal? Latitude { get; init; }

    [JsonPropertyName("longitude")]
    public decimal? Longitude { get; init; }

    [JsonPropertyName("salary_min")]
    public decimal? SalaryMin { get; init; }

    [JsonPropertyName("salary_max")]
    public decimal? SalaryMax { get; init; }

    [JsonPropertyName("salary_is_predicted")]
    public string? SalaryIsPredicted { get; init; }

    [JsonPropertyName("contract_time")]
    public string? ContractTime { get; init; }

    [JsonPropertyName("contract_type")]
    public string? ContractType { get; init; }

    [JsonPropertyName("category")]
    public AdzunaCategoryResponse? Category { get; init; }

    [JsonPropertyName("company")]
    public AdzunaCompanyResponse? Company { get; init; }

    [JsonPropertyName("location")]
    public AdzunaLocationResponse? Location { get; init; }
}

public sealed class AdzunaCompanyResponse
{
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; init; }

    [JsonPropertyName("canonical_name")]
    public string? CanonicalName { get; init; }
}

public sealed class AdzunaCategoryResponse
{
    [JsonPropertyName("tag")]
    public string? Tag { get; init; }

    [JsonPropertyName("label")]
    public string? Label { get; init; }
}

public sealed class AdzunaLocationResponse
{
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; init; }

    [JsonPropertyName("area")]
    public IReadOnlyList<string> Area { get; init; } = [];
}
