using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Commands;

internal static class UserDocumentCommandSupport
{
    public static async Task ClearDefaultDocumentsAsync(
        IdentityDbContext dbContext,
        long userId,
        UserDocumentType documentType,
        CancellationToken cancellationToken)
    {
        var defaultDocuments = await dbContext.UserDocuments
            .Where(document => document.UserAccountId == userId && document.DocumentType == documentType && document.IsDefault)
            .ToListAsync(cancellationToken);

        foreach (var defaultDocument in defaultDocuments)
        {
            defaultDocument.ClearDefault();
        }
    }
}
