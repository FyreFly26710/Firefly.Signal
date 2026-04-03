using System.Net.Http.Json;
using Firefly.Signal.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddDefaultOpenApi();
builder.Services.AddProblemDetails();
builder.Services.Configure<DownstreamOptions>(builder.Configuration.GetSection(DownstreamOptions.SectionName));
builder.Services.AddHttpClient<GatewayDemoClient>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    service = "gateway",
    message = "Firefly Signal backend gateway demo is running."
}));

app.MapGet("/api/demo/topology", (IConfiguration configuration) =>
{
    var options = configuration.GetSection(DownstreamOptions.SectionName).Get<DownstreamOptions>() ?? new DownstreamOptions();
    return Results.Ok(options);
});

app.MapGet("/api/demo/status", async (GatewayDemoClient client, CancellationToken cancellationToken) =>
{
    var status = await client.GetStatusAsync(cancellationToken);
    return Results.Ok(status);
});

app.UseDefaultOpenApi();
app.Run();

internal sealed class DownstreamOptions
{
    public const string SectionName = "Downstream";
    public string IdentityApiBaseUrl { get; init; } = "http://localhost:5081";
    public string JobSearchApiBaseUrl { get; init; } = "http://localhost:5082";
    public string AiApiBaseUrl { get; init; } = "http://localhost:5083";
}

internal sealed class GatewayDemoClient(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<GatewayStatusResponse> GetStatusAsync(CancellationToken cancellationToken)
    {
        var options = configuration.GetSection(DownstreamOptions.SectionName).Get<DownstreamOptions>() ?? new DownstreamOptions();

        var identity = await httpClient.GetFromJsonAsync<object>($"{options.IdentityApiBaseUrl}/", cancellationToken);
        var jobs = await httpClient.GetFromJsonAsync<object>($"{options.JobSearchApiBaseUrl}/", cancellationToken);
        var ai = await httpClient.GetFromJsonAsync<object>($"{options.AiApiBaseUrl}/", cancellationToken);

        return new GatewayStatusResponse("gateway", identity, jobs, ai);
    }
}

internal sealed record GatewayStatusResponse(string Service, object? Identity, object? Jobs, object? Ai);
