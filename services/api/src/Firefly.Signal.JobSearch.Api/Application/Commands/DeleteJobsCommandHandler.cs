using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class DeleteJobsCommandHandler(JobSearchDbContext dbContext) : IRequestHandler<DeleteJobsCommand, DeleteJobsResponse>
{
    public async Task<DeleteJobsResponse> Handle(DeleteJobsCommand request, CancellationToken cancellationToken)
    {
        if (request.Ids.Count == 0)
        {
            return new DeleteJobsResponse(DeletedCount: 0, DeletedIds: [], MissingIds: [], NotHiddenIds: []);
        }

        var requestedIds = request.Ids.Distinct().ToArray();
        var jobs = await dbContext.Jobs
            .Where(job => requestedIds.Contains(job.Id))
            .ToListAsync(cancellationToken);

        var notHiddenIds = jobs.Where(job => !job.IsHidden).Select(job => job.Id).ToArray();
        if (notHiddenIds.Length > 0)
        {
            var existingIds = jobs.Select(job => job.Id).ToHashSet();
            var unresolvedIds = requestedIds.Where(id => !existingIds.Contains(id)).ToArray();

            return new DeleteJobsResponse(
                DeletedCount: 0,
                DeletedIds: [],
                MissingIds: unresolvedIds,
                NotHiddenIds: notHiddenIds);
        }

        foreach (var job in jobs)
        {
            job.MarkDeleted();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var deletedIds = jobs.Select(job => job.Id).ToArray();
        var missingIds = requestedIds.Except(deletedIds).ToArray();

        return new DeleteJobsResponse(
            DeletedCount: deletedIds.Length,
            DeletedIds: deletedIds,
            MissingIds: missingIds,
            NotHiddenIds: []);
    }
}
