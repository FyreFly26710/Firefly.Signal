using Firefly.Signal.Ai.Api.Contracts.Responses;

namespace Firefly.Signal.Ai.Api.Application.Commands;

public abstract record AiChatStreamEvent;

public sealed record AiChatStreamChunkEvent(string Content) : AiChatStreamEvent;

public sealed record AiChatStreamDoneEvent(AiChatResponse Response) : AiChatStreamEvent;
