using Firefly.Signal.Ai.Api.Application.Commands;
using Firefly.Signal.Ai.Domain;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBus.Events.Ai;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Signal.Ai.Api.Application.IntegrationEventHandlers;

public sealed class AiChatRequestedIntegrationEventHandler(
    IServiceScopeFactory scopeFactory) : IIntegrationEventHandler<AiChatRequestedIntegrationEvent>
{
    public async Task HandleAsync(AiChatRequestedIntegrationEvent @event, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new MqAiChatEventCommand
        {
            CorrelationId = @event.CorrelationId,
            CallerService = @event.CallerService,
            ReplyEventType = @event.ReplyEventType,
            Provider = Enum.Parse<AiProvider>(@event.Provider, ignoreCase: true),
            Model = @event.Model,
            SystemPromptMessageId = @event.SystemPromptMessageId,
            UserPrompt = @event.UserPrompt
        }, ct);
    }
}
