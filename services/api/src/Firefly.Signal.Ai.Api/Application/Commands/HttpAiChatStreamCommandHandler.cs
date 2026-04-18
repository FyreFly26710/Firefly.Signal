using System.Runtime.CompilerServices;
using System.Text;
using Firefly.Signal.Ai.Api.Contracts.Responses;
using Firefly.Signal.Ai.Infrastructure.AiProviders;
using Firefly.Signal.Ai.Infrastructure.Persistence;
using MediatR;

namespace Firefly.Signal.Ai.Api.Application.Commands;

public sealed class HttpAiChatStreamCommandHandler(
    AiDbContext db,
    AiProviderResolver resolver) : IRequestHandler<HttpAiChatStreamCommand, IAsyncEnumerable<AiChatStreamEvent>>
{
    public async Task<IAsyncEnumerable<AiChatStreamEvent>> Handle(HttpAiChatStreamCommand command, CancellationToken ct)
    {
        var messages = await AiChatMessageBuilder.BuildAsync(
            db,
            command.SystemPromptMessageId,
            command.History,
            command.UserPrompt,
            ct);

        var aiRequest = Firefly.Signal.Ai.Domain.AiRequest.QueueHttpById(
            command.Model,
            command.SystemPromptMessageId,
            command.UserPrompt);
        db.AiRequests.Add(aiRequest);
        await db.SaveChangesAsync(ct);

        var provider = resolver.Resolve(command.Provider);
        aiRequest.StartProcessing(command.Provider);
        await db.SaveChangesAsync(ct);

        return StreamAsync(aiRequest, provider, command.Model, messages, ct);
    }

    private async IAsyncEnumerable<AiChatStreamEvent> StreamAsync(
        Firefly.Signal.Ai.Domain.AiRequest aiRequest,
        IAiChatProvider provider,
        string model,
        IReadOnlyList<AiProviderMessage> messages,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var contentBuilder = new StringBuilder();
        await using var enumerator = provider.StreamAsync(new AiProviderRequest(model, messages), ct).GetAsyncEnumerator(ct);

        while (true)
        {
            string chunk;

            try
            {
                if (!await enumerator.MoveNextAsync())
                    break;

                chunk = enumerator.Current;
            }
            catch (Exception ex)
            {
                aiRequest.Fail(ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message);
                await db.SaveChangesAsync(ct);
                throw;
            }

            contentBuilder.Append(chunk);
            yield return new AiChatStreamChunkEvent(chunk);
        }

        var aiResponse = Firefly.Signal.Ai.Domain.AiResponse.Create(aiRequest.Id, contentBuilder.ToString());
        db.AiResponses.Add(aiResponse);
        aiRequest.Complete(aiResponse);
        await db.SaveChangesAsync(ct);

        yield return new AiChatStreamDoneEvent(new AiChatResponse
        {
            RequestId = aiRequest.Id,
            ResponseId = aiResponse.Id,
            Content = aiResponse.Message.Content,
            PromptTokens = aiResponse.PromptTokens,
            CompletionTokens = aiResponse.CompletionTokens,
            TotalTokens = aiResponse.TotalTokens,
            GeneratedAtUtc = aiResponse.GeneratedAtUtc
        });
    }
}
