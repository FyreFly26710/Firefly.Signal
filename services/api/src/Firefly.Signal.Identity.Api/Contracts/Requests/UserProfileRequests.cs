namespace Firefly.Signal.Identity.Contracts.Requests;

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
