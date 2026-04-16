using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Services;

namespace Firefly.Signal.Identity.Application.Mappers;

internal static class IdentityResponseMappers
{
    public static AuthenticatedUserResponse ToAuthenticatedUserResponse(UserAccount user)
        => new(
            UserId: user.Id,
            UserAccount: user.UserAccountName,
            DisplayName: user.DisplayName,
            Email: user.Email,
            Role: user.Role);

    public static LoginResponse ToLoginResponse(LoginTokenResult token, UserAccount user)
        => new(
            AccessToken: token.AccessToken,
            TokenType: "Bearer",
            ExpiresAtUtc: token.ExpiresAtUtc,
            User: ToAuthenticatedUserResponse(user));
}
