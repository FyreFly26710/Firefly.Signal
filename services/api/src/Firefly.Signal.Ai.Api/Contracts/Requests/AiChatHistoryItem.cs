namespace Firefly.Signal.Ai.Api.Contracts.Requests;

/// <summary>
/// A single prior conversation turn used to build chat history.
/// </summary>
public sealed record AiChatHistoryItem(long RequestId, long ResponseId);
