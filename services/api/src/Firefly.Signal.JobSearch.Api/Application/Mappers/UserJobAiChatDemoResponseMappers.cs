using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.Application.Mappers;

internal static class UserJobAiChatDemoResponseMappers
{
    public static UserJobAiChatDemoResponse ToResponse(UserJobAiChatDemoRun run) =>
        new(
            Id: run.Id,
            JobPostingId: run.JobPostingId,
            CorrelationId: run.CorrelationId,
            Status: run.Status.ToString(),
            AiResponseId: run.AiResponseId,
            AiResponseContent: run.AiResponseContent);
}
