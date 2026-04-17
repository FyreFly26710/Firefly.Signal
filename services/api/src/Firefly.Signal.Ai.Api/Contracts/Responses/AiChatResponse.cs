namespace Firefly.Signal.Ai.Api.Contracts.Responses;

/// <summary>
/// Response returned from a direct HTTP AI chat completion request.
/// </summary>
public sealed record AiChatResponse
{
    public required long RequestId { get; init; }
    public required string Content { get; init; }
    public int? PromptTokens { get; init; }
    public int? CompletionTokens { get; init; }
    public int? TotalTokens { get; init; }
    public required DateTime GeneratedAtUtc { get; init; }
}
