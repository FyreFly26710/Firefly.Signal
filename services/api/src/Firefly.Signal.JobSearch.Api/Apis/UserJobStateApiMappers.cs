using Firefly.Signal.JobSearch.Application.Commands;

namespace Firefly.Signal.JobSearch.Api.Apis;

internal static class UserJobStateApiMappers
{
    public static SaveJobCommand ToSaveCommand(long jobId, long userId)
        => new(JobId: jobId, UserAccountId: userId);

    public static UnsaveJobCommand ToUnsaveCommand(long jobId, long userId)
        => new(JobId: jobId, UserAccountId: userId);

    public static HideJobForUserCommand ToHideCommand(long jobId, long userId)
        => new(JobId: jobId, UserAccountId: userId);

    public static UnhideJobForUserCommand ToUnhideCommand(long jobId, long userId)
        => new(JobId: jobId, UserAccountId: userId);

    public static StartUserJobAiChatDemoCommand ToStartDemoAiChatCommand(long jobId, long userId)
        => new(JobId: jobId, UserAccountId: userId);
}
