using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Application.Exceptions;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed class SetDefaultUserDocumentCommandHandler(IdentityDbContext dbContext)
    : IRequestHandler<SetDefaultUserDocumentCommand, UserDocumentResponse?>
{
    public async Task<UserDocumentResponse?> Handle(SetDefaultUserDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await dbContext.UserDocuments
            .SingleOrDefaultAsync(
                existingDocument => existingDocument.Id == request.Id && existingDocument.UserAccountId == request.UserId,
                cancellationToken);

        if (document is null)
        {
            return null;
        }

        if (!UserDocumentResponseMappers.SupportsDefaultSelection(document.DocumentType))
        {
            throw new UserDocumentDefaultSelectionNotSupportedException();
        }

        await UserDocumentCommandSupport.ClearDefaultDocumentsAsync(dbContext, request.UserId, document.DocumentType, cancellationToken);
        document.MarkDefault();
        await dbContext.SaveChangesAsync(cancellationToken);
        return UserDocumentResponseMappers.ToUserDocumentResponse(document);
    }
}
