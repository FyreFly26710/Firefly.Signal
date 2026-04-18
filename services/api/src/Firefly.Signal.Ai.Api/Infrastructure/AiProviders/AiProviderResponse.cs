namespace Firefly.Signal.Ai.Infrastructure.AiProviders;

public sealed record AiProviderResponse(
    string Content,
    int? PromptTokens,
    int? CompletionTokens);
