using Firefly.Signal.Ai.Api.Contracts.Requests;
using Firefly.Signal.Ai.Api.Contracts.Responses;
using Firefly.Signal.Ai.Domain;
using MediatR;

namespace Firefly.Signal.Ai.Api.Application.Commands;

public sealed record HttpAiChatCommand : IRequest<AiChatResponse>
{
    public required AiProvider Provider { get; init; }
    public required string Model { get; init; }
    public long? SystemPromptMessageId { get; init; }
    public IReadOnlyList<AiChatHistoryItem> History { get; init; } = [];
    public string? UserPrompt { get; init; }
}
