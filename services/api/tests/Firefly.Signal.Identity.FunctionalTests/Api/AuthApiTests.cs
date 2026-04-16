using System.Net;
using System.Net.Http.Json;
using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.FunctionalTests.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.Identity.FunctionalTests.Api;

[TestClass]
public sealed class AuthApiTests
{
    [TestMethod]
    public async Task Login_WhenCredentialsAreInvalid_ReturnsUnauthorized()
    {
        using var factory = new IdentityApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginUserRequest("missing-user", "bad-password"));

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Login_WhenCredentialsAreValid_ReturnsTokenAndUser()
    {
        using var factory = new IdentityApiFactory();
        var passwordHasher = new PasswordHasher<UserAccount>();
        var user = IdentityTestData.CreateUser(userAccount: "alex");
        user.ChangePassword(passwordHasher.HashPassword(user, "Password123!"));

        await factory.SeedAsync(user);

        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginUserRequest("alex", "Password123!"));

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.IsNotNull(body);
        Assert.IsFalse(string.IsNullOrWhiteSpace(body.AccessToken));
        Assert.AreEqual("Bearer", body.TokenType);
        Assert.AreEqual(user.Id, body.User.UserId);
        Assert.AreEqual("alex", body.User.UserAccount);
    }
}
