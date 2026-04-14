using System.Security.Cryptography;
using Firefly.Signal.Identity.Application;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Persistence;
using Firefly.Signal.Identity.Infrastructure.Services;
using Firefly.Signal.Identity.Infrastructure.Storage;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Firefly.Signal.Identity.Api.Apis;

public static class UserDocumentApi
{
    public static RouteGroupBuilder MapUserDocumentApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users/documents").RequireAuthorization();

        group.MapGet("/", ListAsync);
        group.MapGet("/{id:long}", GetByIdAsync);
        group.MapPost("/", UploadAsync)
            .Accepts<UploadUserDocumentRequest>("multipart/form-data")
            .DisableAntiforgery();
        group.MapPost("/{id:long}/default", SetDefaultAsync);
        group.MapDelete("/{id:long}", DeleteAsync);

        return group;
    }

    private static async Task<Results<Ok<IReadOnlyList<UserDocumentResponse>>, UnauthorizedHttpResult>> ListAsync(
        ICurrentUserContext currentUserContext,
        IdentityDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = currentUserContext.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var documents = await dbContext.UserDocuments
            .Where(x => x.UserAccountId == userId.Value)
            .OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.UploadedAtUtc)
            .Select(x => x.ToResponse())
            .ToListAsync(cancellationToken);

        return TypedResults.Ok<IReadOnlyList<UserDocumentResponse>>(documents);
    }

    private static async Task<Results<Ok<UserDocumentResponse>, NotFound, UnauthorizedHttpResult>> GetByIdAsync(
        long id,
        ICurrentUserContext currentUserContext,
        IdentityDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = currentUserContext.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var document = await dbContext.UserDocuments
            .SingleOrDefaultAsync(x => x.Id == id && x.UserAccountId == userId.Value, cancellationToken);

        return document is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(document.ToResponse());
    }

    private static async Task<Results<Created<UserDocumentResponse>, BadRequest<ProblemDetails>, ValidationProblem, UnauthorizedHttpResult>> UploadAsync(
        [FromForm] UploadUserDocumentRequest request,
        ICurrentUserContext currentUserContext,
        IdentityDbContext dbContext,
        IUserDocumentStorage documentStorage,
        IOptions<UserDocumentStorageOptions> storageOptions,
        CancellationToken cancellationToken)
    {
        var userId = currentUserContext.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var validationErrors = ValidateUploadRequest(request, storageOptions.Value);
        if (validationErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        if (!UserDocumentContractMappings.TryParseDocumentType(request.DocumentType, out var documentType))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["documentType"] = ["Document type must be one of cv, cover-letter, profile-supporting, or other."]
            });
        }

        if (!await dbContext.Users.AnyAsync(x => x.Id == userId.Value, cancellationToken))
        {
            return TypedResults.Unauthorized();
        }

        var file = request.File!;
        await using var uploadStream = new MemoryStream();
        await file.CopyToAsync(uploadStream, cancellationToken);
        var contentBytes = uploadStream.ToArray();
        var checksumSha256 = Convert.ToHexString(SHA256.HashData(contentBytes)).ToLowerInvariant();
        var originalFileName = Path.GetFileName(file.FileName.Trim());
        var displayName = string.IsNullOrWhiteSpace(request.DisplayName)
            ? Path.GetFileNameWithoutExtension(originalFileName)
            : request.DisplayName.Trim();

        var supportsDefaultSelection = UserDocumentContractMappings.SupportsDefaultSelection(documentType);
        if (request.IsDefault && !supportsDefaultSelection)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["isDefault"] = ["Only CV and cover-letter documents support default selection."]
            });
        }

        var hasExistingDefault = supportsDefaultSelection && await dbContext.UserDocuments
            .AnyAsync(
                x => x.UserAccountId == userId.Value
                    && x.DocumentType == documentType
                    && x.IsDefault,
                cancellationToken);

        var isDefault = supportsDefaultSelection && (request.IsDefault || !hasExistingDefault);
        var storedDocument = await documentStorage.UploadAsync(
            new UserDocumentUploadRequest(
                userId.Value,
                request.DocumentType.Trim().ToLowerInvariant(),
                originalFileName,
                file.ContentType.Trim(),
                contentBytes,
                checksumSha256),
            cancellationToken);

        try
        {
            if (isDefault)
            {
                await ClearDefaultDocumentsAsync(userId.Value, documentType, dbContext, cancellationToken);
            }

            var document = UserDocument.Create(
                userId.Value,
                documentType,
                displayName,
                originalFileName,
                storedDocument.StorageKey,
                file.ContentType.Trim(),
                file.Length,
                checksumSha256,
                isDefault);

            dbContext.UserDocuments.Add(document);
            await dbContext.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/api/users/documents/{document.Id}", document.ToResponse());
        }
        catch
        {
            await documentStorage.DeleteAsync(storedDocument.StorageKey, cancellationToken);
            throw;
        }
    }

    private static async Task<Results<Ok<UserDocumentResponse>, NotFound, ValidationProblem, UnauthorizedHttpResult>> SetDefaultAsync(
        long id,
        ICurrentUserContext currentUserContext,
        IdentityDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = currentUserContext.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var document = await dbContext.UserDocuments
            .SingleOrDefaultAsync(x => x.Id == id && x.UserAccountId == userId.Value, cancellationToken);

        if (document is null)
        {
            return TypedResults.NotFound();
        }

        if (!UserDocumentContractMappings.SupportsDefaultSelection(document.DocumentType))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["id"] = ["Only CV and cover-letter documents can be marked as default."]
            });
        }

        await ClearDefaultDocumentsAsync(userId.Value, document.DocumentType, dbContext, cancellationToken);
        document.MarkDefault();
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(document.ToResponse());
    }

    private static async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> DeleteAsync(
        long id,
        ICurrentUserContext currentUserContext,
        IdentityDbContext dbContext,
        IUserDocumentStorage documentStorage,
        CancellationToken cancellationToken)
    {
        var userId = currentUserContext.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var document = await dbContext.UserDocuments
            .SingleOrDefaultAsync(x => x.Id == id && x.UserAccountId == userId.Value, cancellationToken);

        if (document is null)
        {
            return TypedResults.NotFound();
        }

        await documentStorage.DeleteAsync(document.StorageKey, cancellationToken);

        var wasDefault = document.IsDefault;
        var documentType = document.DocumentType;

        document.MarkDeleted();
        document.ClearDefault();

        if (wasDefault && UserDocumentContractMappings.SupportsDefaultSelection(documentType))
        {
            var replacementDocument = await dbContext.UserDocuments
                .Where(x => x.UserAccountId == userId.Value && x.DocumentType == documentType && x.Id != document.Id)
                .OrderByDescending(x => x.UploadedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);

            replacementDocument?.MarkDefault();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task ClearDefaultDocumentsAsync(
        long userId,
        UserDocumentType documentType,
        IdentityDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var defaultDocuments = await dbContext.UserDocuments
            .Where(x => x.UserAccountId == userId && x.DocumentType == documentType && x.IsDefault)
            .ToListAsync(cancellationToken);

        foreach (var defaultDocument in defaultDocuments)
        {
            defaultDocument.ClearDefault();
        }
    }

    private static Dictionary<string, string[]> ValidateUploadRequest(
        UploadUserDocumentRequest request,
        UserDocumentStorageOptions storageOptions)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (request.File is null)
        {
            validationErrors["file"] = ["A document file is required."];
            return validationErrors;
        }

        if (string.IsNullOrWhiteSpace(request.DocumentType))
        {
            validationErrors["documentType"] = ["Document type is required."];
        }

        if (request.File.Length <= 0)
        {
            validationErrors["file"] = ["The uploaded file must not be empty."];
        }

        if (request.File.Length > storageOptions.MaxFileSizeBytes)
        {
            validationErrors["file"] = [$"The uploaded file exceeds the {storageOptions.MaxFileSizeBytes} byte limit."];
        }

        var fileName = Path.GetFileName(request.File.FileName ?? string.Empty);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            validationErrors["fileName"] = ["A valid original file name is required."];
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension) || !storageOptions.AllowedFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            validationErrors["fileExtension"] =
                [$"The uploaded file extension must be one of: {string.Join(", ", storageOptions.AllowedFileExtensions)}."];
        }

        var contentType = request.File.ContentType?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(contentType) || !storageOptions.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            validationErrors["contentType"] =
                [$"The uploaded content type must be one of: {string.Join(", ", storageOptions.AllowedContentTypes)}."];
        }

        return validationErrors;
    }
}
