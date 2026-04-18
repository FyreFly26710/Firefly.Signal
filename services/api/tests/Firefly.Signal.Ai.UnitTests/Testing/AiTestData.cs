using Firefly.Signal.Ai.Domain;

namespace Firefly.Signal.Ai.UnitTests.Testing;

internal static class AiTestData
{
    public static AiMessage SystemPrompt(string content = "You are a helpful assistant.")
        => new(AiMessageType.SystemPrompt, content);

    public static AiMessage UserPrompt(string content = "Hello, what can you do?")
        => new(AiMessageType.UserPrompt, content);

    public static AiMessage AgentResponse(string content = "{\"response\":\"I can help with many things.\"}")
        => new(AiMessageType.AgentResponse, content);
}
