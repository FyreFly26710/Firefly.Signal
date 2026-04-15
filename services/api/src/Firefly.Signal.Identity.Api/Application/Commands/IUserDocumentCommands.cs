using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed record UploadUserDocumentCommand(
    long UserId,
    UserDocumentType DocumentType,
    string DisplayName,
    string OriginalFileName,
    string ContentType,
    long FileSizeBytes,
    string ChecksumSha256,
    bool IsDefault,
    byte[] Content);

public interface IUserDocumentCommands
{
    Task<UserDocumentResponse?> UploadAsync(UploadUserDocumentCommand command, CancellationToken cancellationToken = default);
    Task<UserDocumentResponse?> SetDefaultAsync(long userId, long id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long userId, long id, CancellationToken cancellationToken = default);
}
