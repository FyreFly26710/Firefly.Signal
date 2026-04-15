using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Application.Mappers;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public sealed class UserJobStateCommands(JobSearchDbContext dbContext) : IUserJobStateCommands
{
    public async Task<UserJobStateResponse?> SaveJobAsync(
        long jobId,
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Jobs.AnyAsync(job => job.Id == jobId, cancellationToken))
        {
            return null;
        }

        var state = await UpsertAsync(jobId, userAccountId, cancellationToken);
        state.MarkSaved();
        await dbContext.SaveChangesAsync(cancellationToken);
        return JobApplicationResponseMappers.ToUserJobStateResponse(state);
    }

    public async Task<UserJobStateResponse?> UnsaveJobAsync(
        long jobId,
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Jobs.AnyAsync(job => job.Id == jobId, cancellationToken))
        {
            return null;
        }

        var state = await UpsertAsync(jobId, userAccountId, cancellationToken);
        state.Unsave();
        await dbContext.SaveChangesAsync(cancellationToken);
        return JobApplicationResponseMappers.ToUserJobStateResponse(state);
    }

    public async Task<UserJobStateResponse?> HideJobForUserAsync(
        long jobId,
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Jobs.AnyAsync(job => job.Id == jobId, cancellationToken))
        {
            return null;
        }

        var state = await UpsertAsync(jobId, userAccountId, cancellationToken);
        state.Hide();
        await dbContext.SaveChangesAsync(cancellationToken);
        return JobApplicationResponseMappers.ToUserJobStateResponse(state);
    }

    public async Task<UserJobStateResponse?> UnhideJobForUserAsync(
        long jobId,
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Jobs.AnyAsync(job => job.Id == jobId, cancellationToken))
        {
            return null;
        }

        var state = await UpsertAsync(jobId, userAccountId, cancellationToken);
        state.Unhide();
        await dbContext.SaveChangesAsync(cancellationToken);
        return JobApplicationResponseMappers.ToUserJobStateResponse(state);
    }

    private async Task<UserJobState> UpsertAsync(long jobId, long userAccountId, CancellationToken cancellationToken)
    {
        var state = await dbContext.UserJobStates
            .SingleOrDefaultAsync(existingState => existingState.UserAccountId == userAccountId && existingState.JobPostingId == jobId, cancellationToken);

        if (state is not null)
        {
            return state;
        }

        state = UserJobState.Create(userAccountId, jobId);
        dbContext.UserJobStates.Add(state);
        return state;
    }
}
