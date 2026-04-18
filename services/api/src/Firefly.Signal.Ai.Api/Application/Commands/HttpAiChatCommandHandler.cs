using Firefly.Signal.Ai.Api.Contracts.Responses;
using Firefly.Signal.Ai.Domain;
using Firefly.Signal.Ai.Infrastructure.AiProviders;
using Firefly.Signal.Ai.Infrastructure.Persistence;
using MediatR;

namespace Firefly.Signal.Ai.Api.Application.Commands;

public sealed class HttpAiChatCommandHandler(
    AiDbContext db,
    AiProviderResolver resolver) : IRequestHandler<HttpAiChatCommand, AiChatResponse>
{
    public async Task<AiChatResponse> Handle(HttpAiChatCommand command, CancellationToken ct)
    {
        var messages = await AiChatMessageBuilder.BuildAsync(
            db,
            command.SystemPromptMessageId,
            command.History,
            command.UserPrompt,
            ct);

        var aiRequest = AiRequest.QueueHttpById(command.Model, command.SystemPromptMessageId, command.UserPrompt);
        db.AiRequests.Add(aiRequest);
        await db.SaveChangesAsync(ct);

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

        return new AiChatResponse
        {
            RequestId = aiRequest.Id,
            ResponseId = aiResponse.Id,
            Content = aiResponse.Message.Content,
            PromptTokens = aiResponse.PromptTokens,
            CompletionTokens = aiResponse.CompletionTokens,
            TotalTokens = aiResponse.TotalTokens,
            GeneratedAtUtc = aiResponse.GeneratedAtUtc
        };
    }
}
