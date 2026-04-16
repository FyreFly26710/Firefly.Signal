using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed class UpsertUserProfileCommandHandler(IdentityDbContext dbContext)
    : IRequestHandler<UpsertUserProfileCommand, UserProfileUpsertResult?>
{
    public async Task<UserProfileUpsertResult?> Handle(UpsertUserProfileCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == request.UserId, cancellationToken))
        {
            return null;
        }

        var profile = await dbContext.UserProfiles
            .SingleOrDefaultAsync(existingProfile => existingProfile.UserAccountId == request.UserId, cancellationToken);

        if (profile is null)
        {
            profile = Domain.UserProfile.Create(
                request.UserId,
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

            return new UserProfileUpsertResult(Response: UserProfileResponseMappers.ToUserProfileResponse(profile), Created: true);
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
        return new UserProfileUpsertResult(Response: UserProfileResponseMappers.ToUserProfileResponse(profile), Created: false);
    }
}
