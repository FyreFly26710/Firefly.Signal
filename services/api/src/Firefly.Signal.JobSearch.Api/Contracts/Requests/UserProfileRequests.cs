namespace Firefly.Signal.JobSearch.Contracts.Requests;

public sealed record UpsertUserProfileRequest(
    string? FullName,
    string? PreferredTitle,
    string? PrimaryLocationPostcode,
    string? LinkedInUrl,
    string? GitHubUrl,
    string? PortfolioUrl,
    string? Summary,
    string? SkillsText,
    string? ExperienceText,
    string? PreferencesText);
