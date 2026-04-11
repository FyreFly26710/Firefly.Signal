using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Firefly.Signal.Identity.Application;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Signal.Identity.FunctionalTests;

[TestClass]
public class UserProfileEndpointsTests
{
    [TestMethod]
    public async Task Get_profile_returns_not_found_when_current_user_has_no_profile()
    {
        await using var factory = CreateFactory();
        SeedIdentityData(factory.Services);
        using var client = factory.CreateClient();
        var login = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        var response = await client.GetAsync("/api/users/profile");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task Put_profile_creates_current_user_profile_when_missing()
    {
        await using var factory = CreateFactory();
        SeedIdentityData(factory.Services);
        using var client = factory.CreateClient();
        var login = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        var response = await client.PutAsJsonAsync(
            "/api/users/profile",
            new UserProfileRequest(
                "Ada Lovelace",
                "Senior Developer",
                "sw1a 1aa",
                "https://linkedin.com/in/ada",
                "https://github.com/ada",
                "https://ada.dev",
                "Focused on backend and platform engineering.",
                "C#, .NET, PostgreSQL",
                "Built maintainable systems across product and infrastructure.",
                "{\"preferredJobTypes\":[\"backend\"]}"));

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();

        Assert.IsNotNull(profile);
        Assert.AreEqual("Ada Lovelace", profile.FullName);
        Assert.AreEqual("SW1A 1AA", profile.PrimaryLocationPostcode);
        Assert.AreEqual("{\"preferredJobTypes\":[\"backend\"]}", profile.PreferencesJson);
    }

    [TestMethod]
    public async Task Put_profile_updates_existing_current_user_profile()
    {
        await using var factory = CreateFactory();
        SeedIdentityData(factory.Services);
        SeedExistingProfile(factory.Services);
        using var client = factory.CreateClient();
        var login = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        var response = await client.PutAsJsonAsync(
            "/api/users/profile",
            new UserProfileRequest(
                "Ada Lovelace",
                "Principal Engineer",
                "ec1a 1bb",
                "https://linkedin.com/in/ada-updated",
                "https://github.com/ada",
                "https://ada.dev",
                "Updated summary",
                "C#, .NET, Azure",
                "Updated experience summary",
                "{\"preferredJobTypes\":[\"backend\",\"platform\"]}"));

        response.EnsureSuccessStatusCode();
        var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();

        Assert.IsNotNull(profile);
        Assert.AreEqual("Principal Engineer", profile.PreferredTitle);
        Assert.AreEqual("EC1A 1BB", profile.PrimaryLocationPostcode);

        var getResponse = await client.GetAsync("/api/users/profile");
        getResponse.EnsureSuccessStatusCode();

        var persistedProfile = await getResponse.Content.ReadFromJsonAsync<UserProfileResponse>();
        Assert.IsNotNull(persistedProfile);
        Assert.AreEqual("Updated summary", persistedProfile.Summary);
        Assert.AreEqual("{\"preferredJobTypes\":[\"backend\",\"platform\"]}", persistedProfile.PreferencesJson);
    }

    private static WebApplicationFactory<Firefly.Signal.Identity.Api.Program> CreateFactory()
    {
        var databaseName = $"identity-profile-tests-{Guid.NewGuid():N}";

        return new WebApplicationFactory<Firefly.Signal.Identity.Api.Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.UseSetting("Testing:DatabaseName", databaseName);
            });
    }

    private static async Task<LoginResponse> LoginAsAdminAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginUserRequest("admin", "Admin123!"));
        response.EnsureSuccessStatusCode();

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.IsNotNull(login);
        return login;
    }

    private static void SeedIdentityData(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserAccount>>();

        if (dbContext.Users.Any())
        {
            return;
        }

        var admin = UserAccount.Create("admin", string.Empty, "admin@firefly.local", "Firefly Admin", Roles.Admin);
        admin.ChangePassword(passwordHasher.HashPassword(admin, "Admin123!"));

        dbContext.Users.Add(admin);
        dbContext.SaveChanges();
    }

    private static void SeedExistingProfile(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var admin = dbContext.Users.Single(x => x.UserAccountName == "admin");

        if (dbContext.UserProfiles.Any(x => x.UserAccountId == admin.Id))
        {
            return;
        }

        dbContext.UserProfiles.Add(
            UserProfile.Create(
                admin.Id,
                "Ada Lovelace",
                "Senior Developer",
                "SW1A 1AA",
                "https://linkedin.com/in/ada",
                "https://github.com/ada",
                "https://ada.dev",
                "Original summary",
                "C#, .NET",
                "Original experience",
                "{\"preferredJobTypes\":[\"backend\"]}"));

        dbContext.SaveChanges();
    }
}
