using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;

namespace Firefly.Signal.Identity.Application.Mappers;

internal static class UserDocumentResponseMappers
{
    public static UserDocumentResponse ToUserDocumentResponse(UserDocument document)
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
