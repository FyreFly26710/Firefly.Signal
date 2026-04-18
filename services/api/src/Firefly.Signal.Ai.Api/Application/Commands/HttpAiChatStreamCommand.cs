using MediatR;

namespace Firefly.Signal.Ai.Api.Application.Commands;

public sealed record HttpAiChatStreamCommand : IRequest<IAsyncEnumerable<AiChatStreamEvent>>
{
    public required Firefly.Signal.Ai.Domain.AiProvider Provider { get; init; }
    public required string Model { get; init; }
    public long? SystemPromptMessageId { get; init; }
    public IReadOnlyList<Firefly.Signal.Ai.Api.Contracts.Requests.AiChatHistoryItem> History { get; init; } = [];
    public string? UserPrompt { get; init; }
}
