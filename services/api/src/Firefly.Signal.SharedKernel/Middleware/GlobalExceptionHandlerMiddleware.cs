using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Firefly.Signal.SharedKernel.Middleware;

public sealed class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger,
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (!context.RequestAborted.IsCancellationRequested)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            logger.LogWarning(
                exception,
                "Unhandled exception occurred after the response started for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            throw exception;
        }

        logger.LogError(
            exception,
            "Unhandled exception occurred for {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        context.Response.Clear();

        var problemDetails = CreateProblemDetails(context, exception);
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        var written = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        if (!written)
        {
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: context.RequestAborted);
        }
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        if (exception is ValidationException validationException)
        {
            var validationProblem = new ValidationProblemDetails(ToValidationErrors(validationException))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed",
                Detail = "One or more validation errors occurred.",
                Instance = context.Request.Path
            };

            validationProblem.Extensions["traceId"] = traceId;
            return validationProblem;
        }

        if (exception is BadHttpRequestException badHttpRequestException)
        {
            var badRequestProblem = new ProblemDetails
            {
                Status = badHttpRequestException.StatusCode,
                Title = "Bad request",
                Detail = environment.IsDevelopment()
                    ? badHttpRequestException.Message
                    : "The request could not be processed.",
                Instance = context.Request.Path
            };

            badRequestProblem.Extensions["traceId"] = traceId;
            return badRequestProblem;
        }

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred",
            Detail = environment.IsDevelopment()
                ? exception.Message
                : "The server encountered an unexpected error.",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = traceId;
        return problemDetails;
    }

    private static Dictionary<string, string[]> ToValidationErrors(ValidationException validationException)
        => validationException.Errors
            .GroupBy(
                error => string.IsNullOrWhiteSpace(error.PropertyName) ? string.Empty : error.PropertyName,
                StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray(),
                StringComparer.Ordinal);
}
