using System.Net;
using System.Net.Http.Json;
using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.FunctionalTests.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.Identity.FunctionalTests.Api;

[TestClass]
public sealed class UserProfileApiTests
{
    [TestMethod]
    public async Task GetCurrent_WhenUnauthenticated_ReturnsUnauthorized()
    {
        using var factory = new IdentityApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/users/profile");

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task GetCurrent_WhenProfileDoesNotExist_ReturnsNotFound()
    {
        using var factory = new IdentityApiFactory();
        var user = IdentityTestData.CreateUser();
        await factory.SeedAsync(user);

        using var client = factory.CreateAuthenticatedClient(user.Id);

        var response = await client.GetAsync("/api/users/profile");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task UpsertCurrent_WhenProfileIsMissing_ReturnsCreated()
    {
        using var factory = new IdentityApiFactory();
        var user = IdentityTestData.CreateUser();
        await factory.SeedAsync(user);

        using var client = factory.CreateAuthenticatedClient(user.Id);

        var response = await client.PutAsJsonAsync(
            "/api/users/profile",
            new UserProfileRequest(
                FullName: "Alex Example",
                PreferredTitle: "Senior Engineer",
                PrimaryLocationPostcode: "sw1a 1aa",
                LinkedInUrl: "https://linkedin.example/alex",
                GithubUrl: null,
                PortfolioUrl: null,
                Summary: "Summary",
                SkillsText: "Skills",
                ExperienceText: "Experience",
                PreferencesJson: "{\"remote\":true}"));

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<UserProfileResponse>();

        Assert.IsNotNull(body);
        Assert.AreEqual(user.Id, body.UserAccountId);
        Assert.AreEqual("SW1A 1AA", body.PrimaryLocationPostcode);
    }
}
