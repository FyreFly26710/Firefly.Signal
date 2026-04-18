namespace Firefly.Signal.Ai.Api.Contracts.Responses;

public sealed record AiChatResponse
{
    public required long RequestId { get; init; }
    public required long ResponseId { get; init; }
    public required string Content { get; init; }
    public int? PromptTokens { get; init; }
    public int? CompletionTokens { get; init; }
    public int? TotalTokens { get; init; }
    public required DateTime GeneratedAtUtc { get; init; }
}
