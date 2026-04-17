using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Queries;

public sealed class JobApplicationQueries(JobSearchDbContext dbContext) : IJobApplicationQueries
{
    public async Task<IReadOnlyList<AppliedJobSummaryResponse>> GetAppliedJobsAsync(
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        var applications = await dbContext.JobApplications
            .Where(application => application.UserAccountId == userAccountId)
            .Join(
                dbContext.Jobs,
                application => application.JobPostingId,
                job => job.Id,
                (application, job) => new
                {
                    ApplicationId = application.Id,
                    application.JobPostingId,
                    job.Title,
                    job.Company
                })
            .ToListAsync(cancellationToken);

        if (applications.Count == 0)
        {
            return [];
        }

        var applicationIds = applications.Select(application => application.ApplicationId).ToArray();

        var allEntries = await dbContext.JobApplicationStatusEntries
            .Where(entry => applicationIds.Contains(entry.JobApplicationId))
            .OrderByDescending(entry => entry.StatusAtUtc)
            .ToListAsync(cancellationToken);

        var latestByApplicationId = allEntries
            .GroupBy(entry => entry.JobApplicationId)
            .ToDictionary(group => group.Key, group => group.First());

        var appliedAtByApplicationId = allEntries
            .Where(entry => entry.Status == JobApplicationStatus.Applied)
            .GroupBy(entry => entry.JobApplicationId)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(entry => entry.StatusAtUtc).First().StatusAtUtc);

        return applications.Select(application =>
        {
            latestByApplicationId.TryGetValue(application.ApplicationId, out var latest);
            appliedAtByApplicationId.TryGetValue(application.ApplicationId, out var appliedAtUtc);

            return new AppliedJobSummaryResponse(
                ApplicationId: application.ApplicationId,
                JobPostingId: application.JobPostingId,
                Title: application.Title,
                Company: application.Company,
                CurrentStatus: latest?.Status.ToString() ?? JobApplicationStatus.Applied.ToString(),
                AppliedAtUtc: appliedAtUtc,
                LatestStatusAtUtc: latest?.StatusAtUtc);
        }).ToArray();
    }

    public async Task<AppliedJobDetailResponse?> GetAppliedJobDetailAsync(
        long applicationId,
        long userAccountId,
        CancellationToken cancellationToken = default)
    {
        var application = await dbContext.JobApplications
            .Where(existingApplication => existingApplication.Id == applicationId && existingApplication.UserAccountId == userAccountId)
            .Join(
                dbContext.Jobs,
                existingApplication => existingApplication.JobPostingId,
                job => job.Id,
                (existingApplication, job) => new
                {
                    ApplicationId = existingApplication.Id,
                    existingApplication.JobPostingId,
                    existingApplication.Note,
                    job.Title,
                    Company = job.CompanyDisplayName ?? job.Company
                })
            .SingleOrDefaultAsync(cancellationToken);

        if (application is null)
        {
            return null;
        }

        var entries = await dbContext.JobApplicationStatusEntries
            .Where(entry => entry.JobApplicationId == applicationId)
            .OrderBy(entry => entry.StatusAtUtc)
            .ToListAsync(cancellationToken);

        var appliedAtUtc = entries
            .Where(entry => entry.Status == JobApplicationStatus.Applied)
            .Select(entry => entry.StatusAtUtc)
            .DefaultIfEmpty()
            .First();

        var latestStatusAtUtc = entries.Count > 0
            ? entries.Max(entry => entry.StatusAtUtc)
            : appliedAtUtc;

        return new AppliedJobDetailResponse(
            ApplicationId: application.ApplicationId,
            JobPostingId: application.JobPostingId,
            Title: application.Title,
            Company: application.Company,
            Note: application.Note,
            CurrentStatus: entries.Count > 0
                ? entries.MaxBy(entry => entry.StatusAtUtc)!.Status.ToString()
                : JobApplicationStatus.Applied.ToString(),
            AppliedAtUtc: appliedAtUtc,
            LatestStatusAtUtc: latestStatusAtUtc,
            StatusHistory: entries
                .Select(entry => new JobApplicationStatusEntryResponse(
                    Status: entry.Status.ToString(),
                    RoundNumber: entry.RoundNumber,
                    StatusAtUtc: entry.StatusAtUtc))
                .ToArray());
    }
}
