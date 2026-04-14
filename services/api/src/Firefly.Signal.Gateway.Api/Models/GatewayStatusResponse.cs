namespace Firefly.Signal.Gateway.Api.Models;

internal sealed record GatewayStatusResponse(string Service, object? Identity, object? Jobs, object? Ai);
