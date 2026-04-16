using Firefly.Signal.Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed class DeleteUserCommandHandler(IdentityDbContext dbContext) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(existingUser => existingUser.Id == request.Id, cancellationToken);
        if (user is null)
        {
            return false;
        }

        user.MarkDeleted();
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
