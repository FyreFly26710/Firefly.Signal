namespace Firefly.Signal.Identity.Infrastructure.Storage;

public interface IUserDocumentStorage
{
    Task<StoredUserDocument> UploadAsync(UserDocumentUploadRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken);
}

public sealed record UserDocumentUploadRequest(
    long UserAccountId,
    string DocumentType,
    string OriginalFileName,
    string ContentType,
    byte[] Content,
    string ChecksumSha256);

public sealed record StoredUserDocument(string StorageKey);
