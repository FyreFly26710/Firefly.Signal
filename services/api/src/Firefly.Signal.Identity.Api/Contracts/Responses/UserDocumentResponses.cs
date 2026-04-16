namespace Firefly.Signal.Identity.Contracts.Responses;

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
