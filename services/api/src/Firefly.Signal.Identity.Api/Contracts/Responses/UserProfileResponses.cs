namespace Firefly.Signal.Identity.Contracts.Responses;

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
