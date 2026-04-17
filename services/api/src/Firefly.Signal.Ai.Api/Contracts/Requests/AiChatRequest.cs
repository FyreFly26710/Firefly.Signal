namespace Firefly.Signal.Ai.Api.Contracts.Requests;

/// <summary>
/// HTTP request body for direct AI chat completion.
/// </summary>
public sealed record AiChatRequest
{
    /// <summary>
    /// Model identifier, e.g. "gpt-4o" or "deepseek-chat".
    /// The provider is inferred from this value.
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// Optional system-level instructions for the model.
    /// </summary>
    public string? SystemPrompt { get; init; }

    /// <summary>
    /// The user's message to the model.
    /// </summary>
    public string? UserPrompt { get; init; }

    public int? MaxTokens { get; init; }
    public float? Temperature { get; init; }
}
