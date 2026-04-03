using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Firefly.Signal.Identity.Application;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Firefly.Signal.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");

        group.MapPost("/register", RegisterAsync);
        group.MapPost("/login", LoginAsync);
        group.MapGet("/me", GetCurrentUserAsync).RequireAuthorization();

        return endpoints;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterUserRequest request,
        IdentityDbContext dbContext,
        IPasswordHasher<UserAccount> passwordHasher,
        CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(x => x.UserAccountName == request.UserAccount, cancellationToken))
        {
            return Results.Conflict(new ProblemDetails { Title = "User account already exists." });
        }

        var user = UserAccount.Create(request.UserAccount, string.Empty, request.Email, request.DisplayName, Roles.User);
        user.ChangePassword(passwordHasher.HashPassword(user, request.Password));

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/api/users/{user.Id}", ToResponse(user));
    }

    private static async Task<IResult> LoginAsync(
        LoginUserRequest request,
        IdentityDbContext dbContext,
        IPasswordHasher<UserAccount> passwordHasher,
        IJwtTokenService jwtTokenService,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.UserAccountName == request.UserAccount, cancellationToken);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result is PasswordVerificationResult.Failed)
        {
            return Results.Unauthorized();
        }

        var token = jwtTokenService.CreateToken(user);
        return Results.Ok(new LoginResponse(token.AccessToken, "Bearer", token.ExpiresAtUtc, ToResponse(user)));
    }

    private static async Task<IResult> GetCurrentUserAsync(
        ClaimsPrincipal claimsPrincipal,
        IdentityDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var subject = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(subject, out var userId))
        {
            return Results.Unauthorized();
        }

        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
        return user is null ? Results.NotFound() : Results.Ok(ToResponse(user));
    }

    private static AuthenticatedUserResponse ToResponse(UserAccount user)
        => new(user.Id, user.UserAccountName, user.DisplayName, user.Email, user.Role);
}
