namespace Firefly.Signal.Ai.Infrastructure.AiProviders;

public sealed record AiProviderRequest(
    string Model,
    IReadOnlyList<AiProviderMessage> Messages);
