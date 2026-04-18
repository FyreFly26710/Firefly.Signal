namespace Firefly.Signal.Ai.Infrastructure.AiProviders;

public sealed record AiProviderMessage(string Role, string Content)
{
    public static AiProviderMessage System(string content) => new("system", content);
    public static AiProviderMessage User(string content) => new("user", content);
    public static AiProviderMessage Assistant(string content) => new("assistant", content);
}
