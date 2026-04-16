using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Firefly.Signal.Identity.Infrastructure.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed class DeleteUserDocumentCommandHandler(
    IdentityDbContext dbContext,
    IUserDocumentStorage documentStorage) : IRequestHandler<DeleteUserDocumentCommand, bool>
{
    public async Task<bool> Handle(DeleteUserDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await dbContext.UserDocuments
            .SingleOrDefaultAsync(
                existingDocument => existingDocument.Id == request.Id && existingDocument.UserAccountId == request.UserId,
                cancellationToken);

        if (document is null)
        {
            return false;
        }

        await documentStorage.DeleteAsync(document.StorageKey, cancellationToken);

        var wasDefault = document.IsDefault;
        var documentType = document.DocumentType;

        document.MarkDeleted();
        document.ClearDefault();

        if (wasDefault && UserDocumentResponseMappers.SupportsDefaultSelection(documentType))
        {
            var replacementDocument = await dbContext.UserDocuments
                .Where(existingDocument => existingDocument.UserAccountId == request.UserId
                    && existingDocument.DocumentType == documentType
                    && existingDocument.Id != document.Id)
                .OrderByDescending(existingDocument => existingDocument.UploadedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);

            replacementDocument?.MarkDefault();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
