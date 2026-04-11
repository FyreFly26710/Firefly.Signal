using Firefly.Signal.Identity.Domain;
using Microsoft.AspNetCore.Http;

namespace Firefly.Signal.Identity.Application;

public sealed class UploadUserDocumentRequest
{
    public string DocumentType { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool IsDefault { get; set; }
    public IFormFile? File { get; set; }
}

public sealed record UserDocumentResponse(
    long Id,
    string DocumentType,
    string DisplayName,
    string OriginalFileName,
    string StorageKey,
    string ContentType,
    long FileSizeBytes,
    string ChecksumSha256,
    bool IsDefault,
    bool SupportsDefaultSelection,
    DateTime UploadedAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public static class UserDocumentContractMappings
{
    public static UserDocumentResponse ToResponse(this UserDocument document)
        => new(
            document.Id,
            ToApiDocumentType(document.DocumentType),
            document.DisplayName,
            document.OriginalFileName,
            document.StorageKey,
            document.ContentType,
            document.FileSizeBytes,
            document.ChecksumSha256,
            document.IsDefault,
            SupportsDefaultSelection(document.DocumentType),
            document.UploadedAtUtc,
            document.CreatedAtUtc,
            document.UpdatedAtUtc);

    public static bool TryParseDocumentType(string? value, out UserDocumentType documentType)
    {
        switch (value?.Trim().ToLowerInvariant())
        {
            case "cv":
                documentType = UserDocumentType.Cv;
                return true;
            case "cover-letter":
                documentType = UserDocumentType.CoverLetter;
                return true;
            case "profile-supporting":
                documentType = UserDocumentType.ProfileSupporting;
                return true;
            case "other":
                documentType = UserDocumentType.Other;
                return true;
            default:
                documentType = default;
                return false;
        }
    }

    public static bool SupportsDefaultSelection(UserDocumentType documentType)
        => documentType is UserDocumentType.Cv or UserDocumentType.CoverLetter;

    private static string ToApiDocumentType(UserDocumentType documentType)
        => documentType switch
        {
            UserDocumentType.Cv => "cv",
            UserDocumentType.CoverLetter => "cover-letter",
            UserDocumentType.ProfileSupporting => "profile-supporting",
            UserDocumentType.Other => "other",
            _ => documentType.ToString().ToLowerInvariant()
        };
}
