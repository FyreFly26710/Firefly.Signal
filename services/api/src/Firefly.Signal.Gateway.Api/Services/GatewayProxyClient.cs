using System.Net;
using System.Net.Http.Headers;
using Firefly.Signal.Gateway.Api.Models;
using Firefly.Signal.Gateway.Api.Options;

namespace Firefly.Signal.Gateway.Api.Services;

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
        var options = configuration
            .GetSection(DownstreamOptions.SectionName)
            .Get<DownstreamOptions>() ?? new DownstreamOptions();

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
        var hasBody = request.ContentLength is > 0 || request.Headers.ContainsKey("Transfer-Encoding");

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
