using Firefly.Signal.Ai.Domain;
using Firefly.Signal.Ai.Infrastructure.AiProviders;
using Firefly.Signal.Ai.Infrastructure.Concurrency;
using Firefly.Signal.Ai.Infrastructure.Persistence;
using Firefly.Signal.EventBus;
using Firefly.Signal.EventBus.Events.Ai;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Ai.Api.Application.Commands;

public sealed class MqAiChatEventCommandHandler(
    AiDbContext db,
    AiProviderResolver resolver,
    AiMqThrottle throttle,
    IEventBus eventBus) : IRequestHandler<MqAiChatEventCommand>
{
    public async Task Handle(MqAiChatEventCommand command, CancellationToken ct)
    {
        var aiRequest = await db.AiRequests
            .Include(request => request.UserPromptMessage)
            .Include(request => request.Response)
                .ThenInclude(response => response!.Message)
            .SingleOrDefaultAsync(request => request.CorrelationId == command.CorrelationId, ct);

        if (aiRequest is null)
        {
            aiRequest = AiRequest.QueueFromEventById(
                command.Model,
                command.SystemPromptMessageId,
                command.UserPrompt,
                command.CorrelationId,
                command.CallerService,
                command.ReplyEventType);

            db.AiRequests.Add(aiRequest);
            await db.SaveChangesAsync(ct);
        }
        else if (aiRequest.Status == AiRequestStatus.Completed && aiRequest.Response is not null)
        {
            await PublishCompletedEventAsync(aiRequest, ct);
            return;
        }
        else if (aiRequest.Status == AiRequestStatus.Failed)
        {
            aiRequest.RequeueForRetry();
            await db.SaveChangesAsync(ct);
        }

        using var lease = await throttle.AcquireAsync(ct);

        var messages = await AiChatMessageBuilder.BuildAsync(
            db,
            aiRequest.SystemPromptMessageId,
            [],
            aiRequest.UserPromptMessage?.Content,
            ct);

        var provider = resolver.Resolve(command.Provider);
        if (aiRequest.Status == AiRequestStatus.Queued)
        {
            aiRequest.StartProcessing(command.Provider);
            await db.SaveChangesAsync(ct);
        }
        else if (aiRequest.IsProcessingStale(DateTime.UtcNow, AiRequest.MqProcessingRecoveryWindow))
        {
            aiRequest.RecoverStaleProcessing();
            await db.SaveChangesAsync(ct);
        }

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

        await PublishCompletedEventAsync(aiRequest, ct);
    }

    private async Task PublishCompletedEventAsync(AiRequest aiRequest, CancellationToken ct)
    {
        if (aiRequest.Response is null)
            throw new InvalidOperationException("Cannot publish completion event without a persisted AI response.");

        await eventBus.PublishAsync(new AiChatCompletedIntegrationEvent
        {
            CorrelationId = aiRequest.CorrelationId ?? throw new InvalidOperationException("MQ request is missing CorrelationId."),
            ResponseContent = aiRequest.Response.Message.Content,
            ResponseId = aiRequest.Response.Id,
            PromptTokens = aiRequest.Response.PromptTokens,
            CompletionTokens = aiRequest.Response.CompletionTokens
        }, ct);
    }
}
