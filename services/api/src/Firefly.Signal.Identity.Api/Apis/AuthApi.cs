using Firefly.Signal.Identity.Application;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Firefly.Signal.Identity.Infrastructure.Services;
using Firefly.Signal.SharedKernel.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Api.Apis;

public static class AuthApi
{
    public static RouteGroupBuilder MapAuthApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");

        group.MapPost("/login", LoginAsync);
        group.MapGet("/me", GetCurrentUserAsync).RequireAuthorization();

        return group;
    }

    private static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> LoginAsync(
        LoginUserRequest request,
        IdentityDbContext dbContext,
        IPasswordHasher<UserAccount> passwordHasher,
        IJwtTokenService jwtTokenService,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(x => x.UserAccountName == request.UserAccount, cancellationToken);

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        var passwordVerification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordVerification is PasswordVerificationResult.Failed)
        {
            return TypedResults.Unauthorized();
        }

        var token = jwtTokenService.CreateToken(user);
        return TypedResults.Ok(IdentityMapper.ToLoginResponse(token, user));
    }

    private static async Task<Results<Ok<AuthenticatedUserResponse>, NotFound, UnauthorizedHttpResult>> GetCurrentUserAsync(
        IIdentityService identityService,
        IdentityDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId.Value, cancellationToken);
        return user is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(IdentityMapper.ToAuthenticatedUserResponse(user));
    }
}
