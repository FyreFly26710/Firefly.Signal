using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.JobSearch.Domain;

public sealed class UserProfile : AuditableEntity
{
    private UserProfile()
    {
    }

    public long UserAccountId { get; private set; }
    public string? FullName { get; private set; }
    public string? PreferredTitle { get; private set; }
    public string? PrimaryLocationPostcode { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public string? GitHubUrl { get; private set; }
    public string? PortfolioUrl { get; private set; }
    public string? Summary { get; private set; }
    public string? SkillsText { get; private set; }
    public string? ExperienceText { get; private set; }
    public string? PreferencesText { get; private set; }

    public static UserProfile Create(
        long userAccountId,
        string? fullName,
        string? preferredTitle,
        string? primaryLocationPostcode,
        string? linkedInUrl,
        string? gitHubUrl,
        string? portfolioUrl,
        string? summary,
        string? skillsText,
        string? experienceText,
        string? preferencesText)
    {
        var profile = new UserProfile { UserAccountId = userAccountId };
        profile.Apply(fullName, preferredTitle, primaryLocationPostcode, linkedInUrl, gitHubUrl, portfolioUrl, summary, skillsText, experienceText, preferencesText);
        return profile;
    }

    public void Update(
        string? fullName,
        string? preferredTitle,
        string? primaryLocationPostcode,
        string? linkedInUrl,
        string? gitHubUrl,
        string? portfolioUrl,
        string? summary,
        string? skillsText,
        string? experienceText,
        string? preferencesText)
    {
        Apply(fullName, preferredTitle, primaryLocationPostcode, linkedInUrl, gitHubUrl, portfolioUrl, summary, skillsText, experienceText, preferencesText);
        Touch();
    }

    private void Apply(
        string? fullName,
        string? preferredTitle,
        string? primaryLocationPostcode,
        string? linkedInUrl,
        string? gitHubUrl,
        string? portfolioUrl,
        string? summary,
        string? skillsText,
        string? experienceText,
        string? preferencesText)
    {
        FullName = Normalize(fullName);
        PreferredTitle = Normalize(preferredTitle);
        PrimaryLocationPostcode = NormalizePostcode(primaryLocationPostcode);
        LinkedInUrl = Normalize(linkedInUrl);
        GitHubUrl = Normalize(gitHubUrl);
        PortfolioUrl = Normalize(portfolioUrl);
        Summary = Normalize(summary);
        SkillsText = Normalize(skillsText);
        ExperienceText = Normalize(experienceText);
        PreferencesText = Normalize(preferencesText);
    }

    private static string? Normalize(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private static string? NormalizePostcode(string? value)
    {
        var trimmed = value?.Trim().ToUpperInvariant();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}
