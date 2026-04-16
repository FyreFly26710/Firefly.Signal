using Firefly.Signal.Identity.Application.Commands;
using Firefly.Signal.Identity.FunctionalTests.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.Identity.FunctionalTests.Application;

[TestClass]
public sealed class UpsertUserProfileCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_WhenUserExistsAndProfileMissing_CreatesProfile()
    {
        await using var database = new IdentitySqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var user = IdentityTestData.CreateUser();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var handler = new UpsertUserProfileCommandHandler(dbContext);

        var result = await handler.Handle(
            new UpsertUserProfileCommand(
                UserId: user.Id,
                FullName: "  Alex Example  ",
                PreferredTitle: "  Principal Engineer ",
                PrimaryLocationPostcode: " sw1a 1aa ",
                LinkedInUrl: " https://linkedin.example/alex ",
                GithubUrl: null,
                PortfolioUrl: " ",
                Summary: "  Summary  ",
                SkillsText: "  C#, SQL  ",
                ExperienceText: "  Experience  ",
                PreferencesJson: " "),
            CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Created);
        Assert.AreEqual("Alex Example", result.Response.FullName);
        Assert.AreEqual("SW1A 1AA", result.Response.PrimaryLocationPostcode);
        Assert.AreEqual("{}", result.Response.PreferencesJson);

        var persistedProfile = await dbContext.UserProfiles.SingleAsync(profile => profile.UserAccountId == user.Id);
        Assert.AreEqual("Principal Engineer", persistedProfile.PreferredTitle);
    }

    [TestMethod]
    public async Task Handle_WhenProfileExists_UpdatesExistingProfile()
    {
        await using var database = new IdentitySqliteTestDatabase();
        await using var dbContext = database.CreateDbContext();

        var user = IdentityTestData.CreateUser();
        var profile = IdentityTestData.CreateProfile(user.Id, fullName: "Old Name", preferencesJson: "{\"remote\":false}");

        dbContext.Users.Add(user);
        dbContext.UserProfiles.Add(profile);
        await dbContext.SaveChangesAsync();

        var handler = new UpsertUserProfileCommandHandler(dbContext);

        var result = await handler.Handle(
            new UpsertUserProfileCommand(
                UserId: user.Id,
                FullName: "New Name",
                PreferredTitle: "Staff Engineer",
                PrimaryLocationPostcode: "ec1a 1bb",
                LinkedInUrl: null,
                GithubUrl: null,
                PortfolioUrl: null,
                Summary: "Updated summary",
                SkillsText: "Updated skills",
                ExperienceText: "Updated experience",
                PreferencesJson: "{\"remote\":true}"),
            CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.Created);
        Assert.AreEqual("New Name", result.Response.FullName);
        Assert.AreEqual("{\"remote\":true}", result.Response.PreferencesJson);

        var persistedProfile = await dbContext.UserProfiles.SingleAsync(existing => existing.UserAccountId == user.Id);
        Assert.AreEqual("New Name", persistedProfile.FullName);
        Assert.AreEqual("EC1A 1BB", persistedProfile.PrimaryLocationPostcode);
    }
}
