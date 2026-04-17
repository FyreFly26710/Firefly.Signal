using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class ApplyJobCommandHandler(JobSearchDbContext dbContext) : IRequestHandler<ApplyJobCommand, JobApplicationResponse?>
{
    public async Task<JobApplicationResponse?> Handle(ApplyJobCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Jobs.AnyAsync(job => job.Id == request.JobId, cancellationToken))
        {
            return null;
        }

        var userState = await UserJobStateCommandSupport.UpsertAsync(dbContext, request.JobId, request.UserAccountId, cancellationToken);
        userState.MarkApplied();

        var existing = await dbContext.JobApplications
            .SingleOrDefaultAsync(
                application => application.UserAccountId == request.UserAccountId && application.JobPostingId == request.JobId,
                cancellationToken);

        if (existing is not null)
        {
            var hasAppliedEntry = await dbContext.JobApplicationStatusEntries
                .AnyAsync(
                    entry => entry.JobApplicationId == existing.Id && entry.Status == JobApplicationStatus.Applied,
                    cancellationToken);

            if (!hasAppliedEntry)
            {
                dbContext.JobApplicationStatusEntries.Add(JobApplicationStatusEntry.Create(existing.Id, JobApplicationStatus.Applied));
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return await JobApplicationCommandSupport.BuildApplicationResponseAsync(
                dbContext,
                existing.Id,
                existing.JobPostingId,
                existing.Note,
                cancellationToken);
        }

        var application = JobApplication.Create(request.UserAccountId, request.JobId, request.Note);
        dbContext.JobApplications.Add(application);
        var initialEntry = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.Applied);
        dbContext.JobApplicationStatusEntries.Add(initialEntry);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new JobApplicationResponse(
            Id: application.Id,
            JobPostingId: application.JobPostingId,
            Note: application.Note,
            CurrentStatus: JobApplicationStatus.Applied.ToString(),
            AppliedAtUtc: initialEntry.StatusAtUtc,
            LatestStatusAtUtc: initialEntry.StatusAtUtc,
            StatusHistory:
            [
                new JobApplicationStatusEntryResponse(
                    Status: JobApplicationStatus.Applied.ToString(),
                    RoundNumber: null,
                    StatusAtUtc: initialEntry.StatusAtUtc)
            ]);
    }
}
