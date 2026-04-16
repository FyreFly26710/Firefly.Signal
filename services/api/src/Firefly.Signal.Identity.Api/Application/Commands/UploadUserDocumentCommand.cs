using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;
using MediatR;

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
    byte[] Content) : IRequest<UserDocumentResponse?>;
