using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Application.Mappers;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public sealed class JobApplicationCommands(JobSearchDbContext dbContext) : IJobApplicationCommands
{
    public async Task<JobApplicationResponse?> ApplyJobAsync(
        long jobId,
        long userAccountId,
        string? note,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Jobs.AnyAsync(job => job.Id == jobId, cancellationToken))
        {
            return null;
        }

        var existing = await dbContext.JobApplications
            .SingleOrDefaultAsync(application => application.UserAccountId == userAccountId && application.JobPostingId == jobId, cancellationToken);

        if (existing is not null)
        {
            return await BuildApplicationResponseAsync(existing.Id, existing.JobPostingId, existing.Note, cancellationToken);
        }

        var application = JobApplication.Create(userAccountId, jobId, note);
        var initialEntry = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.Applied);

        dbContext.JobApplications.Add(application);
        dbContext.JobApplicationStatusEntries.Add(initialEntry);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new JobApplicationResponse(
            application.Id,
            application.JobPostingId,
            application.Note,
            JobApplicationStatus.Applied.ToString(),
            [new JobApplicationStatusEntryResponse(JobApplicationStatus.Applied.ToString(), initialEntry.StatusAtUtc)]);
    }

    public async Task<JobApplicationResponse?> AdvanceApplicationStatusAsync(
        long jobId,
        long userAccountId,
        JobApplicationStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        var application = await dbContext.JobApplications
            .SingleOrDefaultAsync(existingApplication => existingApplication.UserAccountId == userAccountId && existingApplication.JobPostingId == jobId, cancellationToken);

        if (application is null)
        {
            return null;
        }

        var entries = await dbContext.JobApplicationStatusEntries
            .Where(entry => entry.JobApplicationId == application.Id)
            .OrderByDescending(entry => entry.StatusAtUtc)
            .ToListAsync(cancellationToken);

        var currentStatus = entries.Count > 0 ? entries[0].Status : JobApplicationStatus.Applied;

        if (currentStatus == JobApplicationStatus.Rejected)
        {
            throw new InvalidOperationException("Cannot advance status from Rejected.");
        }

        if ((int)newStatus <= (int)currentStatus)
        {
            throw new InvalidOperationException($"Cannot transition from {currentStatus} to {newStatus}.");
        }

        var newEntry = JobApplicationStatusEntry.Create(application.Id, newStatus);
        dbContext.JobApplicationStatusEntries.Add(newEntry);
        await dbContext.SaveChangesAsync(cancellationToken);

        entries.Insert(0, newEntry);
        return JobApplicationResponseMappers.ToJobApplicationResponse(application.Id, application.JobPostingId, application.Note, entries);
    }

    public async Task<JobApplicationResponse?> UpdateApplicationNoteAsync(
        long jobId,
        long userAccountId,
        string? note,
        CancellationToken cancellationToken = default)
    {
        var application = await dbContext.JobApplications
            .SingleOrDefaultAsync(existingApplication => existingApplication.UserAccountId == userAccountId && existingApplication.JobPostingId == jobId, cancellationToken);

        if (application is null)
        {
            return null;
        }

        application.UpdateNote(note);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await BuildApplicationResponseAsync(application.Id, application.JobPostingId, application.Note, cancellationToken);
    }

    private async Task<JobApplicationResponse> BuildApplicationResponseAsync(
        long applicationId,
        long jobPostingId,
        string? note,
        CancellationToken cancellationToken)
    {
        var entries = await dbContext.JobApplicationStatusEntries
            .Where(entry => entry.JobApplicationId == applicationId)
            .OrderByDescending(entry => entry.StatusAtUtc)
            .ToListAsync(cancellationToken);

        return JobApplicationResponseMappers.ToJobApplicationResponse(applicationId, jobPostingId, note, entries);
    }
}
