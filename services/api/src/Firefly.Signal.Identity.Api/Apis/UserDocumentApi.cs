using System.Security.Cryptography;
using Firefly.Signal.Identity.Application.Commands;
using Firefly.Signal.Identity.Application.Queries;
using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.SharedKernel.Services;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Firefly.Signal.Identity.Infrastructure.Storage;

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
        IIdentityService identityService,
        IUserDocumentQueries queries,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var documents = await queries.ListAsync(userId.Value, cancellationToken);
        return TypedResults.Ok<IReadOnlyList<UserDocumentResponse>>(documents);
    }

    private static async Task<Results<Ok<UserDocumentResponse>, NotFound, UnauthorizedHttpResult>> GetByIdAsync(
        long id,
        IIdentityService identityService,
        IUserDocumentQueries queries,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var document = await queries.GetByIdAsync(userId.Value, id, cancellationToken);

        return document is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(document);
    }

    private static async Task<Results<Created<UserDocumentResponse>, BadRequest<ProblemDetails>, ValidationProblem, UnauthorizedHttpResult>> UploadAsync(
        [FromForm] UploadUserDocumentRequest request,
        IIdentityService identityService,
        IMediator mediator,
        IOptions<UserDocumentStorageOptions> storageOptions,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var validationErrors = UserDocumentApiMappers.ValidateUploadRequest(request, storageOptions.Value);
        if (validationErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        if (!UserDocumentApiMappers.TryToUserDocumentType(request.DocumentType, out var documentType))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["documentType"] = ["Document type must be one of cv, cover-letter, profile-supporting, or other."]
            });
        }

        var file = request.File!;
        await using var uploadStream = new MemoryStream();
        await file.CopyToAsync(uploadStream, cancellationToken);
        var contentBytes = uploadStream.ToArray();
        var checksumSha256 = Convert.ToHexString(SHA256.HashData(contentBytes)).ToLowerInvariant();
        var supportsDefaultSelection = UserDocumentApiMappers.SupportsDefaultSelection(documentType);
        if (request.IsDefault && !supportsDefaultSelection)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["isDefault"] = ["Only CV and cover-letter documents support default selection."]
            });
        }

        var command = UserDocumentApiMappers.ToUploadCommand(
            userId.Value,
            documentType,
            request.DisplayName,
            file,
            contentBytes,
            checksumSha256,
            request.IsDefault);

        var document = await mediator.Send(command, cancellationToken);
        if (document is null)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Created($"/api/users/documents/{document.Id}", document);
    }

    private static async Task<Results<Ok<UserDocumentResponse>, NotFound, ValidationProblem, UnauthorizedHttpResult>> SetDefaultAsync(
        long id,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        try
        {
            var document = await mediator.Send(UserDocumentApiMappers.ToSetDefaultCommand(userId.Value, id), cancellationToken);
            return document is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(document);
        }
        catch (InvalidOperationException exception)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["id"] = [exception.Message]
            });
        }
    }

    private static async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> DeleteAsync(
        long id,
        IIdentityService identityService,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        return await mediator.Send(UserDocumentApiMappers.ToDeleteCommand(userId.Value, id), cancellationToken)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}
