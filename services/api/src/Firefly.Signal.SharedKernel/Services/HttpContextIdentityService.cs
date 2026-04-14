using System.Security.Claims;

namespace Firefly.Signal.SharedKernel.Services;

public sealed class HttpContextIdentityService(IHttpContextAccessor httpContextAccessor) : IIdentityService
{
    public long? GetUserId()
    {
        var subject = httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value
            ?? httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return long.TryParse(subject, out var userId) ? userId : null;
    }

    public string? GetUserName()
        => httpContextAccessor.HttpContext?.User.Identity?.Name;

    public bool IsAuthenticated()
        => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role)
        => httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}
