using Firefly.Signal.JobSearch.Application;
using Firefly.Signal.JobSearch.Infrastructure.External;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Firefly.Signal.JobSearch.Endpoints;

public static class JobSearchEndpoints
{
    public static IEndpointRouteBuilder MapJobSearchEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/job-search").RequireAuthorization();

        group.MapGet("/search", SearchAsync);

        return endpoints;
    }

    private static async Task<Results<Ok<SearchJobsResponse>, BadRequest<ProblemDetails>, ProblemHttpResult>> SearchAsync(
        [FromQuery] string postcode,
        [FromQuery] string keyword,
        [FromQuery] int pageIndex,
        [FromQuery] int pageSize,
        [FromQuery] string? provider,
        IJobSearchService service,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid postcode",
                Detail = "A postcode is required."
            });
        }

        if (string.IsNullOrWhiteSpace(keyword))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid keyword",
                Detail = "A keyword is required."
            });
        }

        if (!TryParseProvider(provider, out var parsedProvider))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid provider",
                Detail = "The requested job search provider is not supported."
            });
        }

        var request = new SearchJobsRequest(postcode, keyword, Math.Max(pageIndex, 0), pageSize <= 0 ? 20 : pageSize, parsedProvider);
        try
        {
            return TypedResults.Ok(await service.SearchAsync(request, cancellationToken));
        }
        catch (JobSearchProviderException exception)
        {
            return TypedResults.Problem(
                title: "Job search provider unavailable",
                detail: exception.Message,
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }

    private static bool TryParseProvider(string? provider, out JobSearchProviderKind parsedProvider)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            parsedProvider = JobSearchProviderKind.Adzuna;
            return true;
        }

        return Enum.TryParse(provider, ignoreCase: true, out parsedProvider);
    }
}
