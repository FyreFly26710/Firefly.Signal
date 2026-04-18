using Firefly.Signal.Identity.Domain;

namespace Firefly.Signal.Identity.FunctionalTests.Testing;

internal static class IdentityTestData
{
    public static UserAccount CreateUser(
        string userAccount = "alex",
        string passwordHash = "hashed-password",
        string? email = "alex@example.test",
        string? displayName = "Alex Example",
        string role = Roles.User)
        => UserAccount.Create(userAccount, passwordHash, email, displayName, role);
}
