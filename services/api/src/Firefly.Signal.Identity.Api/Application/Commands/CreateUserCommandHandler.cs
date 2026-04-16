using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed class CreateUserCommandHandler(
    IdentityDbContext dbContext,
    IPasswordHasher<UserAccount> passwordHasher) : IRequestHandler<CreateUserCommand, AuthenticatedUserResponse?>
{
    public async Task<AuthenticatedUserResponse?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(user => user.UserAccountName == request.UserAccount, cancellationToken))
        {
            return null;
        }

        var user = UserAccount.Create(request.UserAccount, string.Empty, request.Email, request.DisplayName, request.Role);
        user.ChangePassword(passwordHasher.HashPassword(user, request.Password));

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return IdentityResponseMappers.ToAuthenticatedUserResponse(user);
    }
}
