using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.Identity.Domain;

/// <summary>
/// Stores the user's profile context used by job review and AI-assisted workflows.
/// </summary>
public sealed class UserProfile : AuditableEntity
{
    private UserProfile()
    {
    }

    /// <summary>
    /// Owning account for this profile record.
    /// </summary>
    public long UserAccountId { get; private set; }
    public string? FullName { get; private set; }
    public string? PreferredTitle { get; private set; }
    /// <summary>
    /// Primary UK postcode used as the default location anchor for search and distance calculations.
    /// </summary>
    public string? PrimaryLocationPostcode { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public string? GithubUrl { get; private set; }
    public string? PortfolioUrl { get; private set; }
    public string? Summary { get; private set; }
    /// <summary>
    /// Free-form skills snapshot kept flexible for the MVP.
    /// </summary>
    public string? SkillsText { get; private set; }
    /// <summary>
    /// Free-form experience summary kept flexible for the MVP.
    /// </summary>
    public string? ExperienceText { get; private set; }
    /// <summary>
    /// JSON-backed preference storage for light MVP customization without early over-normalization.
    /// </summary>
    public string PreferencesJson { get; private set; } = "{}";

    public static UserProfile Create(
        long userAccountId,
        string? fullName,
        string? preferredTitle,
        string? primaryLocationPostcode,
        string? linkedInUrl,
        string? githubUrl,
        string? portfolioUrl,
        string? summary,
        string? skillsText,
        string? experienceText,
        string? preferencesJson)
    {
        return new UserProfile
        {
            UserAccountId = userAccountId,
            FullName = Normalize(fullName),
            PreferredTitle = Normalize(preferredTitle),
            PrimaryLocationPostcode = NormalizePostcode(primaryLocationPostcode),
            LinkedInUrl = Normalize(linkedInUrl),
            GithubUrl = Normalize(githubUrl),
            PortfolioUrl = Normalize(portfolioUrl),
            Summary = Normalize(summary),
            SkillsText = Normalize(skillsText),
            ExperienceText = Normalize(experienceText),
            PreferencesJson = NormalizeJson(preferencesJson)
        };
    }

    public void Update(
        string? fullName,
        string? preferredTitle,
        string? primaryLocationPostcode,
        string? linkedInUrl,
        string? githubUrl,
        string? portfolioUrl,
        string? summary,
        string? skillsText,
        string? experienceText,
        string? preferencesJson)
    {
        FullName = Normalize(fullName);
        PreferredTitle = Normalize(preferredTitle);
        PrimaryLocationPostcode = NormalizePostcode(primaryLocationPostcode);
        LinkedInUrl = Normalize(linkedInUrl);
        GithubUrl = Normalize(githubUrl);
        PortfolioUrl = Normalize(portfolioUrl);
        Summary = Normalize(summary);
        SkillsText = Normalize(skillsText);
        ExperienceText = Normalize(experienceText);
        PreferencesJson = NormalizeJson(preferencesJson);
        Touch();
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizePostcode(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    private static string NormalizeJson(string? value) => string.IsNullOrWhiteSpace(value) ? "{}" : value.Trim();
}
