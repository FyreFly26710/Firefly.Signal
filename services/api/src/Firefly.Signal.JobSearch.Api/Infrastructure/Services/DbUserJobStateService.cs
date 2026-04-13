using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public sealed class DbUserJobStateService(JobSearchDbContext dbContext) : IUserJobStateService
{
    public async Task<UserJobStateResponse?> SaveJobAsync(
        long jobId,
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Jobs.AnyAsync(x => x.Id == jobId, cancellationToken))
            return null;

        var state = await UpsertAsync(jobId, userAccountId, cancellationToken);
        state.MarkSaved();
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(state);
    }

    public async Task<UserJobStateResponse?> UnsaveJobAsync(
        long jobId,
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Jobs.AnyAsync(x => x.Id == jobId, cancellationToken))
            return null;

        var state = await UpsertAsync(jobId, userAccountId, cancellationToken);
        state.Unsave();
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(state);
    }

    public async Task<UserJobStateResponse?> HideJobForUserAsync(
        long jobId,
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Jobs.AnyAsync(x => x.Id == jobId, cancellationToken))
            return null;

        var state = await UpsertAsync(jobId, userAccountId, cancellationToken);
        state.Hide();
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(state);
    }

    public async Task<UserJobStateResponse?> UnhideJobForUserAsync(
        long jobId,
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Jobs.AnyAsync(x => x.Id == jobId, cancellationToken))
            return null;

        var state = await UpsertAsync(jobId, userAccountId, cancellationToken);
        state.Unhide();
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(state);
    }

    private async Task<UserJobState> UpsertAsync(long jobId, long userAccountId, CancellationToken cancellationToken)
    {
        var state = await dbContext.UserJobStates
            .SingleOrDefaultAsync(x => x.UserAccountId == userAccountId && x.JobPostingId == jobId, cancellationToken);

        if (state is not null)
            return state;

        state = UserJobState.Create(userAccountId, jobId);
        dbContext.UserJobStates.Add(state);
        return state;
    }

    private static UserJobStateResponse ToResponse(UserJobState state)
        => new(state.JobPostingId, state.IsSaved, state.IsHidden);
}
