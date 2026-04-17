namespace Firefly.Signal.Ai.Infrastructure.AiProviders;

public interface IAiChatProvider
{
    Task<AiProviderResponse> CompleteAsync(AiProviderRequest request, CancellationToken ct = default);

    IAsyncEnumerable<string> StreamAsync(AiProviderRequest request, CancellationToken ct = default);
}
