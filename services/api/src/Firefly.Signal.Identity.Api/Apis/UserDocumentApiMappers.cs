using Firefly.Signal.Identity.Application.Commands;
using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Storage;

namespace Firefly.Signal.Identity.Api.Apis;

internal static class UserDocumentApiMappers
{
    public static bool TryToUserDocumentType(string? value, out UserDocumentType documentType)
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

    public static Dictionary<string, string[]> ValidateUploadRequest(
        UploadUserDocumentRequest request,
        UserDocumentStorageOptions storageOptions)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (request.File is null)
        {
            validationErrors["file"] = ["A document file is required."];
            return validationErrors;
        }

        if (string.IsNullOrWhiteSpace(request.DocumentType))
        {
            validationErrors["documentType"] = ["Document type is required."];
        }

        if (request.File.Length <= 0)
        {
            validationErrors["file"] = ["The uploaded file must not be empty."];
        }

        if (request.File.Length > storageOptions.MaxFileSizeBytes)
        {
            validationErrors["file"] = [$"The uploaded file exceeds the {storageOptions.MaxFileSizeBytes} byte limit."];
        }

        var fileName = Path.GetFileName(request.File.FileName ?? string.Empty);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            validationErrors["fileName"] = ["A valid original file name is required."];
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension) || !storageOptions.AllowedFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            validationErrors["fileExtension"] =
                [$"The uploaded file extension must be one of: {string.Join(", ", storageOptions.AllowedFileExtensions)}."];
        }

        var contentType = request.File.ContentType?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(contentType) || !storageOptions.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            validationErrors["contentType"] =
                [$"The uploaded content type must be one of: {string.Join(", ", storageOptions.AllowedContentTypes)}."];
        }

        return validationErrors;
    }

    public static bool SupportsDefaultSelection(UserDocumentType documentType)
        => UserDocumentResponseMappers.SupportsDefaultSelection(documentType);

    public static UploadUserDocumentCommand ToUploadCommand(
        long userId,
        UserDocumentType documentType,
        string? displayName,
        IFormFile file,
        byte[] content,
        string checksumSha256,
        bool isDefault)
    {
        var originalFileName = Path.GetFileName(file.FileName.Trim());
        var resolvedDisplayName = string.IsNullOrWhiteSpace(displayName)
            ? Path.GetFileNameWithoutExtension(originalFileName)
            : displayName.Trim();

        return new UploadUserDocumentCommand(
            UserId: userId,
            DocumentType: documentType,
            DisplayName: resolvedDisplayName,
            OriginalFileName: originalFileName,
            ContentType: file.ContentType.Trim(),
            FileSizeBytes: file.Length,
            ChecksumSha256: checksumSha256,
            IsDefault: isDefault,
            Content: content);
    }

    public static SetDefaultUserDocumentCommand ToSetDefaultCommand(long userId, long id)
        => new(UserId: userId, Id: id);

    public static DeleteUserDocumentCommand ToDeleteCommand(long userId, long id)
        => new(UserId: userId, Id: id);
}
