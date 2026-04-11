using Firefly.Signal.Identity.Domain;

namespace Firefly.Signal.Identity.Application;

public sealed record UserProfileRequest(
    string? FullName,
    string? PreferredTitle,
    string? PrimaryLocationPostcode,
    string? LinkedInUrl,
    string? GithubUrl,
    string? PortfolioUrl,
    string? Summary,
    string? SkillsText,
    string? ExperienceText,
    string? PreferencesJson);

public sealed record UserProfileResponse(
    long Id,
    long UserAccountId,
    string? FullName,
    string? PreferredTitle,
    string? PrimaryLocationPostcode,
    string? LinkedInUrl,
    string? GithubUrl,
    string? PortfolioUrl,
    string? Summary,
    string? SkillsText,
    string? ExperienceText,
    string PreferencesJson,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public static class UserProfileContractMappings
{
    public static UserProfileResponse ToResponse(this UserProfile profile)
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
