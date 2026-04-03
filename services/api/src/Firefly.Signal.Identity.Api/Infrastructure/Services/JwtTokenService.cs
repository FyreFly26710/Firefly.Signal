using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Firefly.Signal.Identity.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Firefly.Signal.Identity.Infrastructure.Services;

public interface IJwtTokenService
{
    LoginTokenResult CreateToken(UserAccount user);
}

public sealed record LoginTokenResult(string AccessToken, DateTime ExpiresAtUtc);

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public LoginTokenResult CreateToken(UserAccount user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.ExpiresInMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims:
            [
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserAccountName),
                new(ClaimTypes.Name, user.UserAccountName),
                new(ClaimTypes.Role, user.Role)
            ],
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new LoginTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}

public sealed class JwtOptions
{
    public string Issuer { get; init; } = "Firefly.Signal";
    public string Audience { get; init; } = "Firefly.Signal.Client";
    public string SigningKey { get; init; } = "firefly-signal-dev-signing-key-please-change";
    public int ExpiresInMinutes { get; init; } = 120;
}
