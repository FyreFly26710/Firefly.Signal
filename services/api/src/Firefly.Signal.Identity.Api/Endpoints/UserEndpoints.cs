using Firefly.Signal.Identity.Application;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users").RequireAuthorization();

        group.MapGet("/", ListAsync);
        group.MapGet("/{id:long}", GetByIdAsync);
        group.MapPost("/", CreateAsync);
        group.MapPut("/{id:long}", UpdateAsync);
        group.MapDelete("/{id:long}", DeleteAsync);

        return endpoints;
    }

    private static async Task<IResult> ListAsync(IdentityDbContext dbContext, CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .OrderBy(x => x.UserAccountName)
            .Select(x => new AuthenticatedUserResponse(x.Id, x.UserAccountName, x.DisplayName, x.Email, x.Role))
            .ToListAsync(cancellationToken);

        return Results.Ok(users);
    }

    private static async Task<IResult> GetByIdAsync(long id, IdentityDbContext dbContext, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return user is null
            ? Results.NotFound()
            : Results.Ok(new AuthenticatedUserResponse(user.Id, user.UserAccountName, user.DisplayName, user.Email, user.Role));
    }

    private static async Task<IResult> CreateAsync(
        CreateUserRequest request,
        IdentityDbContext dbContext,
        IPasswordHasher<UserAccount> passwordHasher,
        CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(x => x.UserAccountName == request.UserAccount, cancellationToken))
        {
            return Results.Conflict(new ProblemDetails { Title = "User account already exists." });
        }

        var user = UserAccount.Create(request.UserAccount, string.Empty, request.Email, request.DisplayName, request.Role);
        user.ChangePassword(passwordHasher.HashPassword(user, request.Password));

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/api/users/{user.Id}", new AuthenticatedUserResponse(user.Id, user.UserAccountName, user.DisplayName, user.Email, user.Role));
    }

    private static async Task<IResult> UpdateAsync(long id, UpdateUserRequest request, IdentityDbContext dbContext, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return Results.NotFound();
        }

        user.UpdateProfile(request.Email, request.DisplayName, request.Role);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new AuthenticatedUserResponse(user.Id, user.UserAccountName, user.DisplayName, user.Email, user.Role));
    }

    private static async Task<IResult> DeleteAsync(long id, IdentityDbContext dbContext, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return Results.NotFound();
        }

        user.MarkDeleted();
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}
