using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;

namespace Firefly.Signal.Identity.Application.Mappers;

internal static class UserProfileResponseMappers
{
    public static UserProfileResponse ToUserProfileResponse(UserProfile profile)
        => new(
            profile.Id,
            profile.UserAccountId,
            profile.FullName,
            profile.PreferredTitle,
            profile.PrimaryLocationPostcode,
            profile.LinkedInUrl,
            profile.GithubUrl,
            profile.PortfolioUrl,
            profile.Summary,
            profile.SkillsText,
            profile.ExperienceText,
            profile.PreferencesJson,
            profile.CreatedAtUtc,
            profile.UpdatedAtUtc);
}
