using Firefly.Signal.SharedKernel.Domain;

namespace Firefly.Signal.Identity.Domain;

/// <summary>
/// Metadata for a user-owned document such as a CV or cover letter.
/// </summary>
public sealed class UserDocument : AuditableEntity, IAggregateRoot
{
    private UserDocument()
    {
    }

    public long UserAccountId { get; private set; }
    public UserDocumentType DocumentType { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string OriginalFileName { get; private set; } = string.Empty;
    /// <summary>
    /// Storage key or relative file path used to resolve the physical document outside the relational row.
    /// </summary>
    public string StorageKey { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    /// <summary>
    /// Content checksum used to support duplicate detection and safer file management.
    /// </summary>
    public string ChecksumSha256 { get; private set; } = string.Empty;
    /// <summary>
    /// Indicates whether this document is the user's default choice for its document type.
    /// </summary>
    public bool IsDefault { get; private set; }
    public DateTime UploadedAtUtc { get; private set; }

    public static UserDocument Create(
        long userAccountId,
        UserDocumentType documentType,
        string displayName,
        string originalFileName,
        string storageKey,
        string contentType,
        long fileSizeBytes,
        string checksumSha256,
        bool isDefault)
    {
        return new UserDocument
        {
            UserAccountId = userAccountId,
            DocumentType = documentType,
            DisplayName = displayName.Trim(),
            OriginalFileName = originalFileName.Trim(),
            StorageKey = storageKey.Trim(),
            ContentType = contentType.Trim(),
            FileSizeBytes = fileSizeBytes,
            ChecksumSha256 = checksumSha256.Trim(),
            IsDefault = isDefault,
            UploadedAtUtc = DateTime.UtcNow
        };
    }

    public void UpdateMetadata(
        string displayName,
        string originalFileName,
        string storageKey,
        string contentType,
        long fileSizeBytes,
        string checksumSha256)
    {
        DisplayName = displayName.Trim();
        OriginalFileName = originalFileName.Trim();
        StorageKey = storageKey.Trim();
        ContentType = contentType.Trim();
        FileSizeBytes = fileSizeBytes;
        ChecksumSha256 = checksumSha256.Trim();
        Touch();
    }

    public void MarkDefault()
    {
        IsDefault = true;
        Touch();
    }

    public void ClearDefault()
    {
        IsDefault = false;
        Touch();
    }
}
