namespace Firefly.Signal.Ai.Api.Contracts.Responses;

public sealed record AiChatStreamChunkResponse
{
    public required string Content { get; init; }
}
