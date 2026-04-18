namespace Firefly.Signal.JobSearch.Contracts.Responses;

public sealed record UserProfileResponse(
    long Id,
    long UserAccountId,
    string? FullName,
    string? PreferredTitle,
    string? PrimaryLocationPostcode,
    string? LinkedInUrl,
    string? GitHubUrl,
    string? PortfolioUrl,
    string? Summary,
    string? SkillsText,
    string? ExperienceText,
    string? PreferencesText,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
