using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Firefly.Signal.ServiceDefaults;

public static class ServiceCollectionExtensions
{
    public static void AddFireflyServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
    }

    public static void AddDefaultOpenApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    public static void MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/_health");
    }

    public static void UseDefaultOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", $"{app.Environment.ApplicationName} v1");
                options.RoutePrefix = "swagger";
            });
        }
    }
}
