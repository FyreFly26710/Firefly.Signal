using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;

namespace Firefly.Signal.Identity.Application.Mappers;

internal static class UserProfileResponseMappers
{
    public static UserProfileResponse ToUserProfileResponse(UserProfile profile)
        => new(
            Id: profile.Id,
            UserAccountId: profile.UserAccountId,
            FullName: profile.FullName,
            PreferredTitle: profile.PreferredTitle,
            PrimaryLocationPostcode: profile.PrimaryLocationPostcode,
            LinkedInUrl: profile.LinkedInUrl,
            GithubUrl: profile.GithubUrl,
            PortfolioUrl: profile.PortfolioUrl,
            Summary: profile.Summary,
            SkillsText: profile.SkillsText,
            ExperienceText: profile.ExperienceText,
            PreferencesJson: profile.PreferencesJson,
            CreatedAtUtc: profile.CreatedAtUtc,
            UpdatedAtUtc: profile.UpdatedAtUtc);
}
