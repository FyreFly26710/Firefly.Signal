using Firefly.Signal.JobSearch.Application.Mappers;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Commands;

internal static class JobApplicationCommandSupport
{
    public static async Task<JobApplicationResponse> BuildApplicationResponseAsync(
        JobSearchDbContext dbContext,
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
