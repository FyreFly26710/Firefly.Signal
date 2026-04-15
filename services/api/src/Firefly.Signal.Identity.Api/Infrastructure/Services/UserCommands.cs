using Firefly.Signal.Identity.Application.Commands;
using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Infrastructure.Services;

public sealed class UserCommands(
    IdentityDbContext dbContext,
    IPasswordHasher<UserAccount> passwordHasher) : IUserCommands
{
    public async Task<AuthenticatedUserResponse?> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
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

    public async Task<AuthenticatedUserResponse?> UpdateAsync(long id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(existingUser => existingUser.Id == id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        user.UpdateProfile(request.Email, request.DisplayName, request.Role);
        await dbContext.SaveChangesAsync(cancellationToken);
        return IdentityResponseMappers.ToAuthenticatedUserResponse(user);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(existingUser => existingUser.Id == id, cancellationToken);
        if (user is null)
        {
            return false;
        }

        user.MarkDeleted();
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
