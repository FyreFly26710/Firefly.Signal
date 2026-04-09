namespace Firefly.Signal.JobSearch.Infrastructure.External;

public sealed class AdzunaOptions
{
    public const string SectionName = "Adzuna";

    public string BaseUrl { get; init; } = "https://api.adzuna.com";
    public string CountryCode { get; init; } = "gb";
    public string AppId { get; init; } = string.Empty;
    public string AppKey { get; init; } = string.Empty;
}
