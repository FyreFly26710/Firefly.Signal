namespace Firefly.Signal.Ai.Infrastructure.AiProviders;

public sealed record AiProviderResponse(
    string Content,
    string RawJson,
    int? PromptTokens,
    int? CompletionTokens);
