using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Contracts.Requests;

namespace Firefly.Signal.JobSearch.Api.Apis;

internal static class JobApplicationApiMappers
{
    public static bool TryParseStatus(string value, out JobApplicationStatus status)
        => Enum.TryParse(value, ignoreCase: true, out status);

    public static ApplyJobCommand ToApplyCommand(long jobId, long userId, ApplyJobRequest request)
        => new(JobId: jobId, UserAccountId: userId, Note: request.Note);

    public static AdvanceApplicationStatusCommand ToAdvanceStatusCommand(long jobId, long userId, JobApplicationStatus status)
        => new(JobId: jobId, UserAccountId: userId, NewStatus: status);

    public static UpdateApplicationNoteCommand ToUpdateNoteCommand(long jobId, long userId, UpdateApplicationNoteRequest request)
        => new(JobId: jobId, UserAccountId: userId, Note: request.Note);
}
