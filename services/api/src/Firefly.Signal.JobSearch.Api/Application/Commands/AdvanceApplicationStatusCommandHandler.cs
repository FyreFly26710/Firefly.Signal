using Firefly.Signal.JobSearch.Application.Mappers;
using Firefly.Signal.JobSearch.Application.Exceptions;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class AdvanceApplicationStatusCommandHandler(JobSearchDbContext dbContext)
    : IRequestHandler<AdvanceApplicationStatusCommand, JobApplicationResponse?>
{
    public async Task<JobApplicationResponse?> Handle(AdvanceApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var application = await dbContext.JobApplications
            .SingleOrDefaultAsync(
                existingApplication => existingApplication.UserAccountId == request.UserAccountId && existingApplication.JobPostingId == request.JobId,
                cancellationToken);

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
            throw new InvalidApplicationStatusTransitionException("Cannot advance status from Rejected.");
        }

        if ((int)request.NewStatus <= (int)currentStatus)
        {
            throw new InvalidApplicationStatusTransitionException(
                $"Cannot transition from {currentStatus} to {request.NewStatus}.");
        }

        var newEntry = JobApplicationStatusEntry.Create(application.Id, request.NewStatus);
        dbContext.JobApplicationStatusEntries.Add(newEntry);
        await dbContext.SaveChangesAsync(cancellationToken);

        entries.Insert(0, newEntry);
        return JobApplicationResponseMappers.ToJobApplicationResponse(application.Id, application.JobPostingId, application.Note, entries);
    }
}
