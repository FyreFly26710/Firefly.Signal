using Firefly.Signal.JobSearch.Application.Queries;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

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

        return applications.Select(application =>
        {
            latestByApplicationId.TryGetValue(application.ApplicationId, out var latest);

            return new AppliedJobSummaryResponse(
                application.ApplicationId,
                application.JobPostingId,
                application.Title,
                application.Company,
                latest?.Status.ToString() ?? JobApplicationStatus.Applied.ToString(),
                latest?.StatusAtUtc);
        }).ToList();
    }
}
