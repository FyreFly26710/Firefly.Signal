using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;

namespace Firefly.Signal.Identity.Application.Mappers;

internal static class UserDocumentResponseMappers
{
    public static UserDocumentResponse ToUserDocumentResponse(UserDocument document)
        => new(
            Id: document.Id,
            DocumentType: ToApiDocumentType(document.DocumentType),
            DisplayName: document.DisplayName,
            OriginalFileName: document.OriginalFileName,
            StorageKey: document.StorageKey,
            ContentType: document.ContentType,
            FileSizeBytes: document.FileSizeBytes,
            ChecksumSha256: document.ChecksumSha256,
            IsDefault: document.IsDefault,
            SupportsDefaultSelection: SupportsDefaultSelection(document.DocumentType),
            UploadedAtUtc: document.UploadedAtUtc,
            CreatedAtUtc: document.CreatedAtUtc,
            UpdatedAtUtc: document.UpdatedAtUtc);

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
