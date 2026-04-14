using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public sealed class DbJobApplicationService(JobSearchDbContext dbContext) : IJobApplicationService
{
    public async Task<JobApplicationResponse?> ApplyJobAsync(
        long jobId,
        long userAccountId,
        string? note,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Jobs.AnyAsync(x => x.Id == jobId, cancellationToken))
            return null;

        var existing = await dbContext.JobApplications
            .SingleOrDefaultAsync(x => x.UserAccountId == userAccountId && x.JobPostingId == jobId, cancellationToken);

        if (existing is not null)
            return await BuildApplicationResponseAsync(existing.Id, existing.JobPostingId, existing.Note, cancellationToken);

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
            .SingleOrDefaultAsync(x => x.UserAccountId == userAccountId && x.JobPostingId == jobId, cancellationToken);

        if (application is null)
            return null;

        var entries = await dbContext.JobApplicationStatusEntries
            .Where(x => x.JobApplicationId == application.Id)
            .OrderByDescending(x => x.StatusAtUtc)
            .ToListAsync(cancellationToken);

        var currentStatus = entries.Count > 0 ? entries[0].Status : JobApplicationStatus.Applied;

        if (currentStatus == JobApplicationStatus.Rejected)
            throw new InvalidOperationException("Cannot advance status from Rejected.");

        if ((int)newStatus <= (int)currentStatus)
            throw new InvalidOperationException($"Cannot transition from {currentStatus} to {newStatus}.");

        var newEntry = JobApplicationStatusEntry.Create(application.Id, newStatus);
        dbContext.JobApplicationStatusEntries.Add(newEntry);
        await dbContext.SaveChangesAsync(cancellationToken);

        entries.Insert(0, newEntry);
        return BuildApplicationResponse(application.Id, application.JobPostingId, application.Note, entries);
    }

    public async Task<JobApplicationResponse?> UpdateApplicationNoteAsync(
        long jobId,
        long userAccountId,
        string? note,
        CancellationToken cancellationToken = default)
    {
        var application = await dbContext.JobApplications
            .SingleOrDefaultAsync(x => x.UserAccountId == userAccountId && x.JobPostingId == jobId, cancellationToken);

        if (application is null)
            return null;

        application.UpdateNote(note);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await BuildApplicationResponseAsync(application.Id, application.JobPostingId, application.Note, cancellationToken);
    }

    public async Task<IReadOnlyList<AppliedJobSummaryResponse>> GetAppliedJobsAsync(
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        var applications = await dbContext.JobApplications
            .Where(x => x.UserAccountId == userAccountId)
            .Join(dbContext.Jobs, a => a.JobPostingId, j => j.Id, (a, j) => new
            {
                ApplicationId = a.Id,
                a.JobPostingId,
                j.Title,
                j.Company
            })
            .ToListAsync(cancellationToken);

        if (applications.Count == 0)
            return [];

        var applicationIds = applications.Select(x => x.ApplicationId).ToArray();

        var allEntries = await dbContext.JobApplicationStatusEntries
            .Where(x => applicationIds.Contains(x.JobApplicationId))
            .OrderByDescending(x => x.StatusAtUtc)
            .ToListAsync(cancellationToken);

        var latestByApplicationId = allEntries
            .GroupBy(x => x.JobApplicationId)
            .ToDictionary(g => g.Key, g => g.First());

        return applications.Select(a =>
        {
            latestByApplicationId.TryGetValue(a.ApplicationId, out var latest);
            return new AppliedJobSummaryResponse(
                a.ApplicationId,
                a.JobPostingId,
                a.Title,
                a.Company,
                latest?.Status.ToString() ?? JobApplicationStatus.Applied.ToString(),
                latest?.StatusAtUtc);
        }).ToList();
    }

    private async Task<JobApplicationResponse> BuildApplicationResponseAsync(
        long applicationId,
        long jobPostingId,
        string? note,
        CancellationToken cancellationToken)
    {
        var entries = await dbContext.JobApplicationStatusEntries
            .Where(x => x.JobApplicationId == applicationId)
            .OrderByDescending(x => x.StatusAtUtc)
            .ToListAsync(cancellationToken);

        return BuildApplicationResponse(applicationId, jobPostingId, note, entries);
    }

    private static JobApplicationResponse BuildApplicationResponse(
        long applicationId,
        long jobPostingId,
        string? note,
        IReadOnlyList<JobApplicationStatusEntry> entries)
    {
        var currentStatus = entries.Count > 0
            ? entries[0].Status.ToString()
            : JobApplicationStatus.Applied.ToString();

        var history = entries
            .OrderBy(x => x.StatusAtUtc)
            .Select(x => new JobApplicationStatusEntryResponse(x.Status.ToString(), x.StatusAtUtc))
            .ToList();

        return new JobApplicationResponse(applicationId, jobPostingId, note, currentStatus, history);
    }
}
