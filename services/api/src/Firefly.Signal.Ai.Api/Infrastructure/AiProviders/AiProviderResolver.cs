using Firefly.Signal.Ai.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Signal.Ai.Infrastructure.AiProviders;

public sealed class AiProviderResolver(IServiceProvider sp)
{
    public IAiChatProvider Resolve(AiProvider provider) => provider switch
    {
        AiProvider.ChatGpt => sp.GetRequiredKeyedService<IAiChatProvider>(AiProvider.ChatGpt),
        AiProvider.DeepSeek => sp.GetRequiredKeyedService<IAiChatProvider>(AiProvider.DeepSeek),
        _ => throw new ArgumentOutOfRangeException(nameof(provider), $"No provider registered for {provider}.")
    };

    /// <summary>
    /// Infers the provider from the model name and returns the resolved provider along with the
    /// <see cref="AiProvider"/> enum value to store on the request.
    /// </summary>
    public (IAiChatProvider provider, AiProvider providerType) ResolveFromModel(string model)
    {
        var providerType = model.ToLowerInvariant() switch
        {
            var m when m.StartsWith("deepseek") => AiProvider.DeepSeek,
            _ => AiProvider.ChatGpt
        };

        return (Resolve(providerType), providerType);
    }
}
