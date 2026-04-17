using System.Runtime.CompilerServices;
using System.Text.Json;
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
        var messages = BuildMessages(request.Messages);

        var options = BuildOptions(request);
        var completion = await chatClient.CompleteChatAsync(messages, options, ct);

        var rawJson = JsonSerializer.Serialize(completion.Value);
        return new AiProviderResponse(
            completion.Value.Content[0].Text,
            rawJson,
            completion.Value.Usage?.InputTokenCount,
            completion.Value.Usage?.OutputTokenCount);
    }

    public async IAsyncEnumerable<string> StreamAsync(
        AiProviderRequest request,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var chatClient = _client.GetChatClient(request.Model);
        var messages = BuildMessages(request.Messages);
        var options = BuildOptions(request);

        await foreach (var update in chatClient.CompleteChatStreamingAsync(messages, options, ct))
        {
            foreach (var part in update.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(part.Text))
                    yield return part.Text;
            }
        }
    }

    private static List<ChatMessage> BuildMessages(IReadOnlyList<AiProviderMessage> messages)
    {
        return messages.Select<AiProviderMessage, ChatMessage>(m => m.Role switch
        {
            "system" => ChatMessage.CreateSystemMessage(m.Content),
            "user" => ChatMessage.CreateUserMessage(m.Content),
            _ => ChatMessage.CreateUserMessage(m.Content)
        }).ToList();
    }

    private static ChatCompletionOptions? BuildOptions(AiProviderRequest request)
    {
        if (request.MaxTokens is null && request.Temperature is null)
            return null;

        var opts = new ChatCompletionOptions();
        if (request.MaxTokens.HasValue) opts.MaxOutputTokenCount = request.MaxTokens.Value;
        if (request.Temperature.HasValue) opts.Temperature = request.Temperature.Value;
        return opts;
    }
}
