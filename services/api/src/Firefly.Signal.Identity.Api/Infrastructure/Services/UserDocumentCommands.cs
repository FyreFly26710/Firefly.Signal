using Firefly.Signal.Identity.Application.Commands;
using Firefly.Signal.Identity.Application.Mappers;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Firefly.Signal.Identity.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Signal.Identity.Infrastructure.Services;

public sealed class UserDocumentCommands(
    IdentityDbContext dbContext,
    IUserDocumentStorage documentStorage) : IUserDocumentCommands
{
    public async Task<UserDocumentResponse?> UploadAsync(UploadUserDocumentCommand command, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == command.UserId, cancellationToken))
        {
            return null;
        }

        var supportsDefaultSelection = UserDocumentResponseMappers.SupportsDefaultSelection(command.DocumentType);
        var hasExistingDefault = supportsDefaultSelection && await dbContext.UserDocuments
            .AnyAsync(
                document => document.UserAccountId == command.UserId
                    && document.DocumentType == command.DocumentType
                    && document.IsDefault,
                cancellationToken);

        var isDefault = supportsDefaultSelection && (command.IsDefault || !hasExistingDefault);

        var storedDocument = await documentStorage.UploadAsync(
            new UserDocumentUploadRequest(
                command.UserId,
                command.DocumentType.ToString().ToLowerInvariant(),
                command.OriginalFileName,
                command.ContentType,
                command.Content,
                command.ChecksumSha256),
            cancellationToken);

        try
        {
            if (isDefault)
            {
                await ClearDefaultDocumentsAsync(command.UserId, command.DocumentType, cancellationToken);
            }

            var document = UserDocument.Create(
                command.UserId,
                command.DocumentType,
                command.DisplayName,
                command.OriginalFileName,
                storedDocument.StorageKey,
                command.ContentType,
                command.FileSizeBytes,
                command.ChecksumSha256,
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

    public async Task<UserDocumentResponse?> SetDefaultAsync(long userId, long id, CancellationToken cancellationToken = default)
    {
        var document = await dbContext.UserDocuments
            .SingleOrDefaultAsync(existingDocument => existingDocument.Id == id && existingDocument.UserAccountId == userId, cancellationToken);

        if (document is null)
        {
            return null;
        }

        if (!UserDocumentResponseMappers.SupportsDefaultSelection(document.DocumentType))
        {
            throw new InvalidOperationException("Only CV and cover-letter documents can be marked as default.");
        }

        await ClearDefaultDocumentsAsync(userId, document.DocumentType, cancellationToken);
        document.MarkDefault();
        await dbContext.SaveChangesAsync(cancellationToken);
        return UserDocumentResponseMappers.ToUserDocumentResponse(document);
    }

    public async Task<bool> DeleteAsync(long userId, long id, CancellationToken cancellationToken = default)
    {
        var document = await dbContext.UserDocuments
            .SingleOrDefaultAsync(existingDocument => existingDocument.Id == id && existingDocument.UserAccountId == userId, cancellationToken);

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
                .Where(existingDocument => existingDocument.UserAccountId == userId
                    && existingDocument.DocumentType == documentType
                    && existingDocument.Id != document.Id)
                .OrderByDescending(existingDocument => existingDocument.UploadedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);

            replacementDocument?.MarkDefault();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task ClearDefaultDocumentsAsync(
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
