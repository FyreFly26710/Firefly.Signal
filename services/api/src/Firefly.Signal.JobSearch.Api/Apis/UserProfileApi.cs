using Firefly.Signal.JobSearch.Application.Commands;
using Firefly.Signal.JobSearch.Application.Queries;
using Firefly.Signal.JobSearch.Contracts.Requests;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.SharedKernel.Services;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Firefly.Signal.JobSearch.Api.Apis;

public static class UserProfileApi
{
    public static IEndpointRouteBuilder MapUserProfileApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/job-search/profile").RequireAuthorization();

        group.MapGet("/", GetAsync);
        group.MapPut("/", UpsertAsync);

        return endpoints;
    }

    private static async Task<Results<Ok<UserProfileResponse>, NotFound, UnauthorizedHttpResult>> GetAsync(
        [FromServices] IIdentityService identityService,
        [FromServices] IUserProfileQueries queries,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var profile = await queries.GetByUserAccountIdAsync(userId.Value, cancellationToken);
        return profile is null ? TypedResults.NotFound() : TypedResults.Ok(profile);
    }

    private static async Task<Results<Ok<UserProfileResponse>, UnauthorizedHttpResult>> UpsertAsync(
        [FromBody] UpsertUserProfileRequest request,
        [FromServices] IIdentityService identityService,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userId = identityService.GetUserId();
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        var command = new UpsertUserProfileCommand(
            userId.Value,
            request.FullName,
            request.PreferredTitle,
            request.PrimaryLocationPostcode,
            request.LinkedInUrl,
            request.GitHubUrl,
            request.PortfolioUrl,
            request.Summary,
            request.SkillsText,
            request.ExperienceText,
            request.PreferencesText);

        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
