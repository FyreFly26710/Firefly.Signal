using Firefly.Signal.Ai.Api.Application.Commands;
using Firefly.Signal.Ai.Api.Contracts.Requests;

namespace Firefly.Signal.Ai.Api.Apis;

internal static class AiApiMappers
{
    public static HttpAiChatCommand ToCommand(this AiChatRequest request) =>
        new()
        {
            Provider = request.Provider,
            Model = request.Model,
            SystemPromptMessageId = request.SystemPromptMessageId,
            History = request.History,
            UserPrompt = request.UserPrompt
        };

    public static HttpAiChatStreamCommand ToStreamCommand(this AiChatRequest request) =>
        new()
        {
            Provider = request.Provider,
            Model = request.Model,
            SystemPromptMessageId = request.SystemPromptMessageId,
            History = request.History,
            UserPrompt = request.UserPrompt
        };
}
