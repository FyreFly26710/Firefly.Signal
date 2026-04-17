using Firefly.Signal.SharedKernel.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Signal.SharedKernel.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddFireflyExceptionHandling(this IServiceCollection services)
        => services;

    public static IApplicationBuilder UseFireflyExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
}
