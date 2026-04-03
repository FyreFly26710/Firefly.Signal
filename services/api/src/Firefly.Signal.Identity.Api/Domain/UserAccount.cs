using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.Identity.Domain;

public sealed class UserAccount : AuditableEntity, IAggregateRoot
{
    private UserAccount()
    {
    }

    public string UserAccountName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? DisplayName { get; private set; }
    public string Role { get; private set; } = Roles.User;

    public static UserAccount Create(string userAccountName, string passwordHash, string? email, string? displayName, string role)
    {
        return new UserAccount
        {
            UserAccountName = userAccountName.Trim(),
            PasswordHash = passwordHash,
            Email = Normalize(email),
            DisplayName = Normalize(displayName),
            Role = string.IsNullOrWhiteSpace(role) ? Roles.User : role.Trim().ToLowerInvariant()
        };
    }

    public void UpdateProfile(string? email, string? displayName, string? role)
    {
        Email = Normalize(email);
        DisplayName = Normalize(displayName);
        if (!string.IsNullOrWhiteSpace(role))
        {
            Role = role.Trim().ToLowerInvariant();
        }

        Touch();
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        Touch();
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public static class Roles
{
    public const string Admin = "admin";
    public const string User = "user";
}
