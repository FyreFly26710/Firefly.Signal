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

        var existing = await dbContext.JobApplications
            .SingleOrDefaultAsync(
                application => application.UserAccountId == request.UserAccountId && application.JobPostingId == request.JobId,
                cancellationToken);

        if (existing is not null)
        {
            return await JobApplicationCommandSupport.BuildApplicationResponseAsync(
                dbContext,
                existing.Id,
                existing.JobPostingId,
                existing.Note,
                cancellationToken);
        }

        var application = JobApplication.Create(request.UserAccountId, request.JobId, request.Note);
        var initialEntry = JobApplicationStatusEntry.Create(application.Id, JobApplicationStatus.Applied);

        dbContext.JobApplications.Add(application);
        dbContext.JobApplicationStatusEntries.Add(initialEntry);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new JobApplicationResponse(
            Id: application.Id,
            JobPostingId: application.JobPostingId,
            Note: application.Note,
            CurrentStatus: JobApplicationStatus.Applied.ToString(),
            StatusHistory: [new JobApplicationStatusEntryResponse(Status: JobApplicationStatus.Applied.ToString(), StatusAtUtc: initialEntry.StatusAtUtc)]);
    }
}
