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
public class IdentityApiTests
{
    [TestMethod]
    public async Task Login_returns_token_for_seeded_admin_user()
    {
        await using var factory = CreateFactory();
        SeedIdentityData(factory.Services);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginUserRequest("admin", "Admin123!"));

        response.EnsureSuccessStatusCode();
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.IsNotNull(login);
        Assert.IsFalse(string.IsNullOrWhiteSpace(login.AccessToken));
        Assert.AreEqual("admin", login.User.UserAccount);
        Assert.AreEqual(Roles.Admin, login.User.Role);
    }

    [TestMethod]
    public async Task Me_returns_current_user_when_bearer_token_is_valid()
    {
        await using var factory = CreateFactory();
        SeedIdentityData(factory.Services);
        using var client = factory.CreateClient();
        var login = await LoginAsAdminAsync(client);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);

        var response = await client.GetAsync("/api/auth/me");

        response.EnsureSuccessStatusCode();
        var currentUser = await response.Content.ReadFromJsonAsync<AuthenticatedUserResponse>();

        Assert.IsNotNull(currentUser);
        Assert.AreEqual("admin", currentUser.UserAccount);
        Assert.AreEqual(Roles.Admin, currentUser.Role);
    }

    [TestMethod]
    public async Task Register_endpoint_is_not_available_for_simple_auth_flow()
    {
        await using var factory = CreateFactory();
        SeedIdentityData(factory.Services);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest("new-user", "Password123!", null, null));

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static WebApplicationFactory<Firefly.Signal.Identity.Api.Program> CreateFactory()
    {
        var databaseName = $"identity-tests-{Guid.NewGuid():N}";

        return new WebApplicationFactory<Firefly.Signal.Identity.Api.Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.UseSetting("Testing:DatabaseName", databaseName);
                builder.ConfigureServices(_ =>
                {
                });
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
}
