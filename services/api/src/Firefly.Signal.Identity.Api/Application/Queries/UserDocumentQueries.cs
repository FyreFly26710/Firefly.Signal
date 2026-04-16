using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Application.Queries;

public sealed class UserDocumentQueries(IdentityDbContext dbContext) : IUserDocumentQueries
{
    public async Task<IReadOnlyList<UserDocumentResponse>> ListAsync(long userId, CancellationToken cancellationToken = default)
        => await dbContext.UserDocuments
            .Where(document => document.UserAccountId == userId)
            .OrderByDescending(document => document.IsDefault)
            .ThenByDescending(document => document.UploadedAtUtc)
            .Select(document => UserDocumentResponseMappers.ToUserDocumentResponse(document))
            .ToListAsync(cancellationToken);

    public async Task<UserDocumentResponse?> GetByIdAsync(long userId, long id, CancellationToken cancellationToken = default)
    {
        var document = await dbContext.UserDocuments
            .SingleOrDefaultAsync(existingDocument => existingDocument.Id == id && existingDocument.UserAccountId == userId, cancellationToken);

        return document is null ? null : UserDocumentResponseMappers.ToUserDocumentResponse(document);
    }
}
