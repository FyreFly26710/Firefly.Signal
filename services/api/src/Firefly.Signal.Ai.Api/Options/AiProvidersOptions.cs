namespace Firefly.Signal.Ai.Api.Options;

public sealed class AiProvidersOptions
{
    public const string SectionName = "AiProviders";

    public string ChatGptApiKey { get; init; } = string.Empty;
    public string DeepSeekApiKey { get; init; } = string.Empty;
}
