using Firefly.Signal.Identity.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.Identity.UnitTests.Domain;

[TestClass]
public sealed class UserProfileTests
{
    [TestMethod]
    public void Create_NormalizesPostcodeAndDefaultsPreferencesJson()
    {
        var profile = UserProfile.Create(
            userAccountId: 42,
            fullName: "  Alex Example  ",
            preferredTitle: "  Senior Engineer  ",
            primaryLocationPostcode: " sw1a 1aa ",
            linkedInUrl: "  https://linkedin.example/alex  ",
            githubUrl: null,
            portfolioUrl: " ",
            summary: "  Focused on backend systems  ",
            skillsText: null,
            experienceText: "  Lots of delivery work  ",
            preferencesJson: "   ");

        Assert.AreEqual("Alex Example", profile.FullName);
        Assert.AreEqual("Senior Engineer", profile.PreferredTitle);
        Assert.AreEqual("SW1A 1AA", profile.PrimaryLocationPostcode);
        Assert.AreEqual("https://linkedin.example/alex", profile.LinkedInUrl);
        Assert.IsNull(profile.PortfolioUrl);
        Assert.AreEqual("Focused on backend systems", profile.Summary);
        Assert.AreEqual("Lots of delivery work", profile.ExperienceText);
        Assert.AreEqual("{}", profile.PreferencesJson);
    }
}
