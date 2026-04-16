namespace Firefly.Signal.Gateway.Api.Options;

internal sealed class GatewayCorsOptions
{
    public const string SectionName = "Cors:AllowedOrigins";
}

internal static class GatewayCorsPolicy
{
    public const string Frontend = "Frontend";
}

internal static class LocalCorsOrigins
{
    public static readonly string[] All =
    [
        "http://localhost:5173",
        "http://127.0.0.1:5173",
        "http://localhost:4173",
        "http://127.0.0.1:4173"
    ];
}
