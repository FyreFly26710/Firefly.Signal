namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "Firefly.Signal";
    public string Audience { get; init; } = "Firefly.Signal.Client";
    public string SigningKey { get; init; } = "firefly-signal-dev-signing-key-please-change";
}
