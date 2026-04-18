using Firefly.Signal.Ai.Domain;
using MediatR;

namespace Firefly.Signal.Ai.Api.Application.Commands;

public sealed record MqAiChatEventCommand : IRequest
{
    public required string CorrelationId { get; init; }
    public required string CallerService { get; init; }
    public required string ReplyEventType { get; init; }
    public required AiProvider Provider { get; init; }
    public required string Model { get; init; }
    public required long SystemPromptMessageId { get; init; }
    public string? UserPrompt { get; init; }
}
