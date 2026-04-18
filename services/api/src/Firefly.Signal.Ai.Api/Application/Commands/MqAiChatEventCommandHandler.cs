using Firefly.Signal.Ai.Domain;
using Firefly.Signal.Ai.Infrastructure.AiProviders;
using Firefly.Signal.Ai.Infrastructure.Concurrency;
using Firefly.Signal.Ai.Infrastructure.Persistence;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBus.Events.Ai;
using MediatR;

namespace Firefly.Signal.Ai.Api.Application.Commands;

public sealed class MqAiChatEventCommandHandler(
    AiDbContext db,
    AiProviderResolver resolver,
    AiMqThrottle throttle,
    IEventBus eventBus) : IRequestHandler<MqAiChatEventCommand>
{
    public async Task Handle(MqAiChatEventCommand command, CancellationToken ct)
    {
        var aiRequest = AiRequest.QueueFromEventById(
            command.Model,
            command.SystemPromptMessageId,
            command.UserPrompt,
            command.CorrelationId,
            command.CallerService,
            command.ReplyEventType);

        db.AiRequests.Add(aiRequest);
        await db.SaveChangesAsync(ct);

        using var lease = await throttle.AcquireAsync(ct);

        var messages = await AiChatMessageBuilder.BuildAsync(
            db,
            command.SystemPromptMessageId,
            [],
            command.UserPrompt,
            ct);

        var provider = resolver.Resolve(command.Provider);
        aiRequest.StartProcessing(command.Provider);
        await db.SaveChangesAsync(ct);

        AiProviderResponse providerResult;
        AiResponse aiResponse;

        try
        {
            providerResult = await provider.CompleteAsync(new AiProviderRequest(command.Model, messages), ct);
            aiResponse = AiResponse.Create(aiRequest.Id, providerResult.Content, providerResult.PromptTokens, providerResult.CompletionTokens);
            db.AiResponses.Add(aiResponse);
            aiRequest.Complete(aiResponse);
            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            aiRequest.Fail(ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message);
            await db.SaveChangesAsync(ct);
            throw;
        }

        await eventBus.PublishAsync(new AiChatCompletedIntegrationEvent
        {
            CorrelationId = command.CorrelationId,
            ResponseContent = providerResult.Content,
            ResponseId = aiResponse.Id,
            PromptTokens = aiResponse.PromptTokens,
            CompletionTokens = aiResponse.CompletionTokens
        }, ct);
    }
}
