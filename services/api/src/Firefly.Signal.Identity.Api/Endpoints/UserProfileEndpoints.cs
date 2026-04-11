using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Firefly.Signal.Identity.Application;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Endpoints;

public static class UserProfileEndpoints
{
    public static IEndpointRouteBuilder MapUserProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users/profile").RequireAuthorization();

        group.MapGet("/", GetCurrentAsync);
        group.MapPut("/", UpsertCurrentAsync);

        return endpoints;
    }

    private static async Task<IResult> GetCurrentAsync(
        ClaimsPrincipal claimsPrincipal,
        IdentityDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var profile = await dbContext.UserProfiles
            .SingleOrDefaultAsync(x => x.UserAccountId == userId.Value, cancellationToken);

        return profile is null ? Results.NotFound() : Results.Ok(profile.ToResponse());
    }

    private static async Task<IResult> UpsertCurrentAsync(
        UserProfileRequest request,
        ClaimsPrincipal claimsPrincipal,
        IdentityDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(claimsPrincipal);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        if (!await dbContext.Users.AnyAsync(x => x.Id == userId.Value, cancellationToken))
        {
            return Results.Unauthorized();
        }

        var profile = await dbContext.UserProfiles
            .SingleOrDefaultAsync(x => x.UserAccountId == userId.Value, cancellationToken);

        if (profile is null)
        {
            profile = UserProfile.Create(
                userId.Value,
                request.FullName,
                request.PreferredTitle,
                request.PrimaryLocationPostcode,
                request.LinkedInUrl,
                request.GithubUrl,
                request.PortfolioUrl,
                request.Summary,
                request.SkillsText,
                request.ExperienceText,
                request.PreferencesJson);

            dbContext.UserProfiles.Add(profile);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Results.Created("/api/users/profile", profile.ToResponse());
        }

        profile.Update(
            request.FullName,
            request.PreferredTitle,
            request.PrimaryLocationPostcode,
            request.LinkedInUrl,
            request.GithubUrl,
            request.PortfolioUrl,
            request.Summary,
            request.SkillsText,
            request.ExperienceText,
            request.PreferencesJson);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(profile.ToResponse());
    }

    private static long? GetCurrentUserId(ClaimsPrincipal claimsPrincipal)
    {
        var subject = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        return long.TryParse(subject, out var userId) ? userId : null;
    }
}
