using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Commands;

internal static class UserJobStateCommandSupport
{
    public static async Task<UserJobState> UpsertAsync(
        JobSearchDbContext dbContext,
        long jobId,
        long userAccountId,
        CancellationToken cancellationToken)
    {
        var state = await dbContext.UserJobStates
            .SingleOrDefaultAsync(
                existingState => existingState.UserAccountId == userAccountId && existingState.JobPostingId == jobId,
                cancellationToken);

        if (state is not null)
        {
            return state;
        }

        state = UserJobState.Create(userAccountId, jobId);
        dbContext.UserJobStates.Add(state);
        return state;
    }
}
