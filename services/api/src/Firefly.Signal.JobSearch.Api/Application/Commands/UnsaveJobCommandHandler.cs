using Firefly.Signal.JobSearch.Application.Mappers;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class UnsaveJobCommandHandler(JobSearchDbContext dbContext) : IRequestHandler<UnsaveJobCommand, UserJobStateResponse?>
{
    public async Task<UserJobStateResponse?> Handle(UnsaveJobCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Jobs.AnyAsync(job => job.Id == request.JobId, cancellationToken))
        {
            return null;
        }

        var state = await UserJobStateCommandSupport.UpsertAsync(dbContext, request.JobId, request.UserAccountId, cancellationToken);
        state.Unsave();
        await dbContext.SaveChangesAsync(cancellationToken);
        return JobApplicationResponseMappers.ToUserJobStateResponse(state);
    }
}
