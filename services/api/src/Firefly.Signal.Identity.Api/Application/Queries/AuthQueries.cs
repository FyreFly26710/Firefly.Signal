using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Firefly.Signal.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Queries;

public sealed class AuthQueries(
    IdentityDbContext dbContext,
    IPasswordHasher<UserAccount> passwordHasher,
    IJwtTokenService jwtTokenService) : IAuthQueries
{
    public async Task<LoginResponse?> AuthenticateAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(existingUser => existingUser.UserAccountName == request.UserAccount, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var passwordVerification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordVerification is PasswordVerificationResult.Failed)
        {
            return null;
        }

        var token = jwtTokenService.CreateToken(user);
        return IdentityResponseMappers.ToLoginResponse(token, user);
    }

    public async Task<AuthenticatedUserResponse?> GetCurrentUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(existingUser => existingUser.Id == userId, cancellationToken);
        return user is null ? null : IdentityResponseMappers.ToAuthenticatedUserResponse(user);
    }
}
