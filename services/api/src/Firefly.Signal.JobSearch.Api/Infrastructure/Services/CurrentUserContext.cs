using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Firefly.Signal.JobSearch.Infrastructure.Services;

public interface ICurrentUserContext
{
    long? GetUserId();
}

internal sealed class HttpContextCurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public long? GetUserId()
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;
        if (claimsPrincipal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var subject = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        return long.TryParse(subject, out var userId) ? userId : null;
    }
}
