using System.Net;
using System.Net.Http.Json;
using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.FunctionalTests.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.JobSearch.FunctionalTests.Api;

[TestClass]
public sealed class UserProfileApiTests
{
    [TestMethod]
    public async Task GetProfile_WhenUnauthenticated_ReturnsUnauthorized()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/job-search/profile");

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task GetProfile_WhenNoProfileExists_ReturnsNotFound()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateAuthenticatedClient(userId: 42);

        var response = await client.GetAsync("/api/job-search/profile");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task UpsertProfile_WhenUnauthenticated_ReturnsUnauthorized()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/job-search/profile", new UpsertUserProfileRequest(
            FullName: "Alex",
            PreferredTitle: null,
            PrimaryLocationPostcode: null,
            LinkedInUrl: null,
            GitHubUrl: null,
            PortfolioUrl: null,
            Summary: null,
            SkillsText: null,
            ExperienceText: null,
            PreferencesText: null));

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task UpsertProfile_WhenNoProfileExists_CreatesAndReturnsProfile()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateAuthenticatedClient(userId: 42);

        var response = await client.PutAsJsonAsync("/api/job-search/profile", new UpsertUserProfileRequest(
            FullName: "Alex Example",
            PreferredTitle: "Senior Engineer",
            PrimaryLocationPostcode: "sw1a 1aa",
            LinkedInUrl: "https://linkedin.com/in/alex",
            GitHubUrl: "https://github.com/alex",
            PortfolioUrl: null,
            Summary: "# Summary\nExperienced engineer.",
            SkillsText: "- C#\n- .NET",
            ExperienceText: "## Experience\n5 years at Acme.",
            PreferencesText: "Remote only."));

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<UserProfileResponse>();

        Assert.IsNotNull(body);
        Assert.AreEqual(42, body.UserAccountId);
        Assert.AreEqual("Alex Example", body.FullName);
        Assert.AreEqual("Senior Engineer", body.PreferredTitle);
        Assert.AreEqual("SW1A 1AA", body.PrimaryLocationPostcode);
        Assert.AreEqual("# Summary\nExperienced engineer.", body.Summary);
        Assert.AreEqual("- C#\n- .NET", body.SkillsText);
    }

    [TestMethod]
    public async Task UpsertProfile_WhenProfileExists_UpdatesAndReturnsProfile()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateAuthenticatedClient(userId: 42);

        await client.PutAsJsonAsync("/api/job-search/profile", new UpsertUserProfileRequest(
            FullName: "Alex Example",
            PreferredTitle: "Senior Engineer",
            PrimaryLocationPostcode: "sw1a 1aa",
            LinkedInUrl: null,
            GitHubUrl: null,
            PortfolioUrl: null,
            Summary: "Original summary.",
            SkillsText: null,
            ExperienceText: null,
            PreferencesText: null));

        var updateResponse = await client.PutAsJsonAsync("/api/job-search/profile", new UpsertUserProfileRequest(
            FullName: "Alex Updated",
            PreferredTitle: "Staff Engineer",
            PrimaryLocationPostcode: "ec1a 1bb",
            LinkedInUrl: null,
            GitHubUrl: null,
            PortfolioUrl: null,
            Summary: "Updated summary.",
            SkillsText: "- C#\n- Go",
            ExperienceText: null,
            PreferencesText: null));

        Assert.AreEqual(HttpStatusCode.OK, updateResponse.StatusCode);

        var body = await updateResponse.Content.ReadFromJsonAsync<UserProfileResponse>();

        Assert.IsNotNull(body);
        Assert.AreEqual("Alex Updated", body.FullName);
        Assert.AreEqual("Staff Engineer", body.PreferredTitle);
        Assert.AreEqual("EC1A 1BB", body.PrimaryLocationPostcode);
        Assert.AreEqual("Updated summary.", body.Summary);
        Assert.AreEqual("- C#\n- Go", body.SkillsText);
    }

    [TestMethod]
    public async Task GetProfile_WhenProfileExists_ReturnsProfile()
    {
        using var factory = new JobSearchApiFactory();
        using var client = factory.CreateAuthenticatedClient(userId: 42);

        await client.PutAsJsonAsync("/api/job-search/profile", new UpsertUserProfileRequest(
            FullName: "Alex Example",
            PreferredTitle: "Senior Engineer",
            PrimaryLocationPostcode: "SW1A 1AA",
            LinkedInUrl: null,
            GitHubUrl: null,
            PortfolioUrl: null,
            Summary: "My summary.",
            SkillsText: null,
            ExperienceText: null,
            PreferencesText: null));

        var getResponse = await client.GetAsync("/api/job-search/profile");

        Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

        var body = await getResponse.Content.ReadFromJsonAsync<UserProfileResponse>();

        Assert.IsNotNull(body);
        Assert.AreEqual(42, body.UserAccountId);
        Assert.AreEqual("Alex Example", body.FullName);
        Assert.AreEqual("Senior Engineer", body.PreferredTitle);
    }
}
