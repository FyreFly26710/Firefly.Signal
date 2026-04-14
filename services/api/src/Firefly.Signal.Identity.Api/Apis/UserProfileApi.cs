using Firefly.Signal.Identity.Application;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Firefly.Signal.SharedKernel.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Api.Apis;

public static class UserProfileApi
{
    public static RouteGroupBuilder MapUserProfileApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users/profile").RequireAuthorization();

        group.MapGet("/", GetCurrentAsync);
        group.MapPut("/", UpsertCurrentAsync);

        return group;
    }

    private static async Task<Results<Ok<UserProfileResponse>, NotFound, UnauthorizedHttpResult>> GetCurrentAsync(
        IIdentityService identityService,
        IdentityDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var profile = await dbContext.UserProfiles
            .SingleOrDefaultAsync(x => x.UserAccountId == userId.Value, cancellationToken);

        return profile is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(UserProfileMapper.ToUserProfileResponse(profile));
    }

    private static async Task<Results<Created<UserProfileResponse>, Ok<UserProfileResponse>, UnauthorizedHttpResult>> UpsertCurrentAsync(
        UserProfileRequest request,
        IIdentityService identityService,
        IdentityDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        if (!await dbContext.Users.AnyAsync(x => x.Id == userId.Value, cancellationToken))
        {
            return TypedResults.Unauthorized();
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
            return TypedResults.Created("/api/users/profile", UserProfileMapper.ToUserProfileResponse(profile));
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
        return TypedResults.Ok(UserProfileMapper.ToUserProfileResponse(profile));
    }
}
