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

    public static UserProfile CreateProfile(
        long userAccountId,
        string? fullName = "Alex Example",
        string? postcode = "SW1A 1AA",
        string? preferencesJson = "{\"remote\":true}")
        => UserProfile.Create(
            userAccountId,
            fullName,
            preferredTitle: "Senior Engineer",
            primaryLocationPostcode: postcode,
            linkedInUrl: "https://linkedin.example/alex",
            githubUrl: "https://github.example/alex",
            portfolioUrl: null,
            summary: "Profile summary",
            skillsText: "C#, SQL, React",
            experienceText: "Experience summary",
            preferencesJson: preferencesJson);

    public static UserDocument CreateDocument(
        long userAccountId,
        UserDocumentType documentType,
        string displayName,
        bool isDefault)
        => UserDocument.Create(
            userAccountId,
            documentType,
            displayName,
            $"{displayName.Replace(' ', '-').ToLowerInvariant()}.pdf",
            $"testing/user-documents/{userAccountId}/{Guid.NewGuid():N}.pdf",
            "application/pdf",
            fileSizeBytes: 128,
            checksumSha256: Guid.NewGuid().ToString("N").PadLeft(64, 'a')[..64],
            isDefault);
}
