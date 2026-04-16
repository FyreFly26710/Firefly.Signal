using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class HideJobsCommandHandler(JobSearchDbContext dbContext) : IRequestHandler<HideJobsCommand, HideJobsResponse>
{
    public async Task<HideJobsResponse> Handle(HideJobsCommand request, CancellationToken cancellationToken)
    {
        if (request.Ids.Count == 0)
        {
            return new HideJobsResponse(HiddenCount: 0, HiddenIds: [], MissingIds: []);
        }

        var requestedIds = request.Ids.Distinct().ToArray();
        var jobs = await dbContext.Jobs
            .Where(job => requestedIds.Contains(job.Id))
            .ToListAsync(cancellationToken);

        foreach (var job in jobs.Where(existingJob => !existingJob.IsHidden))
        {
            job.Hide();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var hiddenIds = jobs.Select(job => job.Id).ToArray();
        var missingIds = requestedIds.Except(hiddenIds).ToArray();

        return new HideJobsResponse(
            HiddenCount: hiddenIds.Length,
            HiddenIds: hiddenIds,
            MissingIds: missingIds);
    }
}
