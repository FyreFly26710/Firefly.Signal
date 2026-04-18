using System.Runtime.CompilerServices;
using Firefly.Signal.Ai.Api.Options;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace Firefly.Signal.Ai.Infrastructure.AiProviders;

public sealed class ChatGptProvider : IAiChatProvider
{
    private readonly OpenAIClient _client;

    public ChatGptProvider(IOptions<AiProvidersOptions> options)
    {
        _client = new OpenAIClient(options.Value.ChatGptApiKey);
    }

    public async Task<AiProviderResponse> CompleteAsync(AiProviderRequest request, CancellationToken ct = default)
    {
        var chatClient = _client.GetChatClient(request.Model);
        var completion = await chatClient.CompleteChatAsync(ToSdkMessages(request.Messages), cancellationToken: ct);
        return new AiProviderResponse(
            completion.Value.Content[0].Text,
            completion.Value.Usage?.InputTokenCount,
            completion.Value.Usage?.OutputTokenCount);
    }

    public async IAsyncEnumerable<string> StreamAsync(
        AiProviderRequest request,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var chatClient = _client.GetChatClient(request.Model);
        await foreach (var update in chatClient.CompleteChatStreamingAsync(ToSdkMessages(request.Messages), cancellationToken: ct))
        {
            foreach (var part in update.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(part.Text))
                    yield return part.Text;
            }
        }
    }

    private static List<ChatMessage> ToSdkMessages(IReadOnlyList<AiProviderMessage> messages) =>
        messages.Select<AiProviderMessage, ChatMessage>(m => m.Role switch
        {
            "system" => ChatMessage.CreateSystemMessage(m.Content),
            "assistant" => ChatMessage.CreateAssistantMessage(m.Content),
            _ => ChatMessage.CreateUserMessage(m.Content)
        }).ToList();
}
