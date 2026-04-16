using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed class UpdateUserCommandHandler(IdentityDbContext dbContext) : IRequestHandler<UpdateUserCommand, AuthenticatedUserResponse?>
{
    public async Task<AuthenticatedUserResponse?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(existingUser => existingUser.Id == request.Id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        user.UpdateProfile(request.Email, request.DisplayName, request.Role);
        await dbContext.SaveChangesAsync(cancellationToken);
        return IdentityResponseMappers.ToAuthenticatedUserResponse(user);
    }
}
