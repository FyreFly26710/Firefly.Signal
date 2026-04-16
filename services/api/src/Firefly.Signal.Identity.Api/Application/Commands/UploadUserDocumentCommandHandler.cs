using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Firefly.Signal.Identity.Infrastructure.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed class UploadUserDocumentCommandHandler(
    IdentityDbContext dbContext,
    IUserDocumentStorage documentStorage) : IRequestHandler<UploadUserDocumentCommand, UserDocumentResponse?>
{
    public async Task<UserDocumentResponse?> Handle(UploadUserDocumentCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == request.UserId, cancellationToken))
        {
            return null;
        }

        var supportsDefaultSelection = UserDocumentResponseMappers.SupportsDefaultSelection(request.DocumentType);
        var hasExistingDefault = supportsDefaultSelection && await dbContext.UserDocuments
            .AnyAsync(
                document => document.UserAccountId == request.UserId
                    && document.DocumentType == request.DocumentType
                    && document.IsDefault,
                cancellationToken);

        var isDefault = supportsDefaultSelection && (request.IsDefault || !hasExistingDefault);

        var storedDocument = await documentStorage.UploadAsync(
            new UserDocumentUploadRequest(
                UserAccountId: request.UserId,
                DocumentType: request.DocumentType.ToString().ToLowerInvariant(),
                OriginalFileName: request.OriginalFileName,
                ContentType: request.ContentType,
                Content: request.Content,
                ChecksumSha256: request.ChecksumSha256),
            cancellationToken);

        try
        {
            if (isDefault)
            {
                await UserDocumentCommandSupport.ClearDefaultDocumentsAsync(dbContext, request.UserId, request.DocumentType, cancellationToken);
            }

            var document = UserDocument.Create(
                request.UserId,
                request.DocumentType,
                request.DisplayName,
                request.OriginalFileName,
                storedDocument.StorageKey,
                request.ContentType,
                request.FileSizeBytes,
                request.ChecksumSha256,
                isDefault);

            dbContext.UserDocuments.Add(document);
            await dbContext.SaveChangesAsync(cancellationToken);

            return UserDocumentResponseMappers.ToUserDocumentResponse(document);
        }
        catch
        {
            await documentStorage.DeleteAsync(storedDocument.StorageKey, cancellationToken);
            throw;
        }
    }
}
