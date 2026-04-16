using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class UpdateApplicationNoteCommandHandler(JobSearchDbContext dbContext)
    : IRequestHandler<UpdateApplicationNoteCommand, JobApplicationResponse?>
{
    public async Task<JobApplicationResponse?> Handle(UpdateApplicationNoteCommand request, CancellationToken cancellationToken)
    {
        var application = await dbContext.JobApplications
            .SingleOrDefaultAsync(
                existingApplication => existingApplication.UserAccountId == request.UserAccountId && existingApplication.JobPostingId == request.JobId,
                cancellationToken);

        if (application is null)
        {
            return null;
        }

        application.UpdateNote(request.Note);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await JobApplicationCommandSupport.BuildApplicationResponseAsync(
            dbContext,
            application.Id,
            application.JobPostingId,
            application.Note,
            cancellationToken);
    }
}
