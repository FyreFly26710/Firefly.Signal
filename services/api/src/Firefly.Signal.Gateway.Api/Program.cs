using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Firefly.Signal.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddFireflyServiceDefaults();
builder.AddDefaultOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection(CorsOptions.SectionName).Get<string[]>() ??
            [
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "http://localhost:4173",
                "http://127.0.0.1:4173"
            ];

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.Configure<DownstreamOptions>(builder.Configuration.GetSection(DownstreamOptions.SectionName));
builder.Services.AddHttpClient<GatewayDemoClient>();
builder.Services.AddHttpClient<GatewayProxyClient>();

var app = builder.Build();

app.UseCors("Frontend");
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

MapProxyRoute(app, "/api/auth/{**catchAll}", DownstreamService.Identity);
MapProxyRoute(app, "/api/auth", DownstreamService.Identity);
MapProxyRoute(app, "/api/users/{**catchAll}", DownstreamService.Identity);
MapProxyRoute(app, "/api/users", DownstreamService.Identity);
MapProxyRoute(app, "/api/job-search/{**catchAll}", DownstreamService.JobSearch);
MapProxyRoute(app, "/api/job-search", DownstreamService.JobSearch);
MapProxyRoute(app, "/api/ai/{**catchAll}", DownstreamService.Ai);
MapProxyRoute(app, "/api/ai", DownstreamService.Ai);

app.UseDefaultOpenApi();
app.Run();

static void MapProxyRoute(WebApplication app, string pattern, DownstreamService service)
{
    app.MapMethods(pattern, [HttpMethods.Options], () => Results.NoContent());
    app.MapMethods(pattern, GatewayProxyClient.AllowedMethods, (HttpContext context, GatewayProxyClient client)
        => client.ForwardAsync(context, service));
}

internal enum DownstreamService
{
    Identity,
    JobSearch,
    Ai
}

internal sealed class DownstreamOptions
{
    public const string SectionName = "Downstream";
    public string IdentityApiBaseUrl { get; init; } = "http://localhost:5081";
    public string JobSearchApiBaseUrl { get; init; } = "http://localhost:5082";
    public string AiApiBaseUrl { get; init; } = "http://localhost:5083";

    public string GetBaseUrl(DownstreamService service)
        => service switch
        {
            DownstreamService.Identity => IdentityApiBaseUrl,
            DownstreamService.JobSearch => JobSearchApiBaseUrl,
            DownstreamService.Ai => AiApiBaseUrl,
            _ => throw new ArgumentOutOfRangeException(nameof(service), service, "Unknown downstream service.")
        };
}

internal sealed class CorsOptions
{
    public const string SectionName = "Cors:AllowedOrigins";
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

internal sealed class GatewayProxyClient(HttpClient httpClient, IConfiguration configuration)
{
    private static readonly HashSet<string> HopByHopHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Connection",
        "Keep-Alive",
        "Proxy-Authenticate",
        "Proxy-Authorization",
        "TE",
        "Trailer",
        "Transfer-Encoding",
        "Upgrade",
        "Host"
    };

    public static readonly string[] AllowedMethods =
    [
        HttpMethods.Get,
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete,
        HttpMethods.Head
    ];

    public async Task ForwardAsync(HttpContext context, DownstreamService service)
    {
        var options = configuration.GetSection(DownstreamOptions.SectionName).Get<DownstreamOptions>() ?? new DownstreamOptions();
        var targetUri = BuildTargetUri(context.Request, options.GetBaseUrl(service));

        using var downstreamRequest = CreateDownstreamRequest(context.Request, targetUri);
        using var downstreamResponse = await httpClient.SendAsync(
            downstreamRequest,
            HttpCompletionOption.ResponseHeadersRead,
            context.RequestAborted);

        await CopyDownstreamResponseAsync(context.Response, downstreamResponse, context.RequestAborted);
    }

    private static Uri BuildTargetUri(HttpRequest request, string baseUrl)
    {
        var builder = new UriBuilder(baseUrl)
        {
            Path = request.Path.ToString(),
            Query = request.QueryString.HasValue ? request.QueryString.Value : string.Empty
        };

        return builder.Uri;
    }

    private static HttpRequestMessage CreateDownstreamRequest(HttpRequest request, Uri targetUri)
    {
        var downstreamRequest = new HttpRequestMessage(new HttpMethod(request.Method), targetUri);

        var hasBody =
            request.ContentLength is > 0 ||
            request.Headers.ContainsKey("Transfer-Encoding");

        if (hasBody)
        {
            downstreamRequest.Content = new StreamContent(request.Body);
        }

        foreach (var header in request.Headers)
        {
            if (HopByHopHeaders.Contains(header.Key))
            {
                continue;
            }

            if (!downstreamRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) &&
                downstreamRequest.Content is not null)
            {
                downstreamRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        if (downstreamRequest.Content is not null &&
            request.ContentType is not null &&
            downstreamRequest.Content.Headers.ContentType is null)
        {
            downstreamRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ContentType);
        }

        return downstreamRequest;
    }

    private static async Task CopyDownstreamResponseAsync(
        HttpResponse response,
        HttpResponseMessage downstreamResponse,
        CancellationToken cancellationToken)
    {
        response.StatusCode = (int)downstreamResponse.StatusCode;

        foreach (var header in downstreamResponse.Headers)
        {
            if (HopByHopHeaders.Contains(header.Key))
            {
                continue;
            }

            response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in downstreamResponse.Content.Headers)
        {
            if (HopByHopHeaders.Contains(header.Key))
            {
                continue;
            }

            response.Headers[header.Key] = header.Value.ToArray();
        }

        response.Headers.Remove("transfer-encoding");

        if (downstreamResponse.StatusCode != HttpStatusCode.NoContent)
        {
            await downstreamResponse.Content.CopyToAsync(response.Body, cancellationToken);
        }
    }
}

internal sealed record GatewayStatusResponse(string Service, object? Identity, object? Jobs, object? Ai);
