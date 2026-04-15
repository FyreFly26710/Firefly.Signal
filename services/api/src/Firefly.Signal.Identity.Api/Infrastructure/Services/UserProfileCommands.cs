using Firefly.Signal.Identity.Application.Commands;
using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Infrastructure.Services;

public sealed class UserProfileCommands(IdentityDbContext dbContext) : IUserProfileCommands
{
    public async Task<UserProfileUpsertResult?> UpsertCurrentAsync(
        long userId,
        UserProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            return null;
        }

        var profile = await dbContext.UserProfiles
            .SingleOrDefaultAsync(existingProfile => existingProfile.UserAccountId == userId, cancellationToken);

        if (profile is null)
        {
            profile = Domain.UserProfile.Create(
                userId,
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

            return new UserProfileUpsertResult(UserProfileResponseMappers.ToUserProfileResponse(profile), true);
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
        return new UserProfileUpsertResult(UserProfileResponseMappers.ToUserProfileResponse(profile), false);
    }
}
