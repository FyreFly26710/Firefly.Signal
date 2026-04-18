using Firefly.Signal.Ai.Api.Contracts.Requests;
using Firefly.Signal.Ai.Infrastructure.AiProviders;
using Firefly.Signal.Ai.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Ai.Api.Application;

/// <summary>
/// Builds a flat list of provider messages from persisted IDs and the current user prompt.
/// Message order: [system?, user1?, assistant1?, ..., current_user?]
/// </summary>
public static class AiChatMessageBuilder
{
    public static async Task<List<AiProviderMessage>> BuildAsync(
        AiDbContext db,
        long? systemPromptMessageId,
        IReadOnlyList<AiChatHistoryItem> history,
        string? currentUserPrompt,
        CancellationToken ct)
    {
        var messages = new List<AiProviderMessage>();

        if (systemPromptMessageId.HasValue)
        {
            var systemMsg = await db.AiMessages.FindAsync([systemPromptMessageId.Value], ct);
            if (systemMsg != null)
                messages.Add(AiProviderMessage.System(systemMsg.Content));
        }

        if (history.Count > 0)
        {
            var requestIds = history.Select(h => h.RequestId).ToHashSet();
            var responseIds = history.Select(h => h.ResponseId).ToHashSet();

            var requests = await db.AiRequests
                .Where(r => requestIds.Contains(r.Id))
                .Include(r => r.UserPromptMessage)
                .ToDictionaryAsync(r => r.Id, ct);

            var responses = await db.AiResponses
                .Where(r => responseIds.Contains(r.Id))
                .Include(r => r.Message)
                .ToDictionaryAsync(r => r.Id, ct);

            foreach (var item in history)
            {
                if (requests.TryGetValue(item.RequestId, out var req) && req.UserPromptMessage != null)
                    messages.Add(AiProviderMessage.User(req.UserPromptMessage.Content));

                if (responses.TryGetValue(item.ResponseId, out var resp))
                    messages.Add(AiProviderMessage.Assistant(resp.Message.Content));
            }
        }

        if (!string.IsNullOrWhiteSpace(currentUserPrompt))
            messages.Add(AiProviderMessage.User(currentUserPrompt));

        return messages;
    }
}
