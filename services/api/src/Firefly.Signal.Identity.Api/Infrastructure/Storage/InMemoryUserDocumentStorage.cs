using System.Collections.Concurrent;

namespace Firefly.Signal.Identity.Infrastructure.Storage;

internal sealed class InMemoryUserDocumentStorage : IUserDocumentStorage
{
    private readonly ConcurrentDictionary<string, StoredUserDocumentContent> _documents = new();

    public Task<StoredUserDocument> UploadAsync(UserDocumentUploadRequest request, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(request.OriginalFileName);
        var storageKey = $"testing/user-documents/{request.UserAccountId}/{Guid.NewGuid():N}{extension}";

        _documents[storageKey] = new StoredUserDocumentContent(
            request.ContentType,
            request.ChecksumSha256,
            request.Content.ToArray());

        return Task.FromResult(new StoredUserDocument(storageKey));
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        _documents.TryRemove(storageKey, out _);
        return Task.CompletedTask;
    }
}

internal sealed record StoredUserDocumentContent(string ContentType, string ChecksumSha256, byte[] Content);
