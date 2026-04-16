using Firefly.Signal.Identity.Application.Commands;
using Firefly.Signal.Identity.Contracts.Requests;

namespace Firefly.Signal.Identity.Api.Apis;

internal static class UserProfileApiMappers
{
    public static UpsertUserProfileCommand ToUpsertCommand(long userId, UserProfileRequest request)
        => new(
            UserId: userId,
            FullName: request.FullName,
            PreferredTitle: request.PreferredTitle,
            PrimaryLocationPostcode: request.PrimaryLocationPostcode,
            LinkedInUrl: request.LinkedInUrl,
            GithubUrl: request.GithubUrl,
            PortfolioUrl: request.PortfolioUrl,
            Summary: request.Summary,
            SkillsText: request.SkillsText,
            ExperienceText: request.ExperienceText,
            PreferencesJson: request.PreferencesJson);
}
