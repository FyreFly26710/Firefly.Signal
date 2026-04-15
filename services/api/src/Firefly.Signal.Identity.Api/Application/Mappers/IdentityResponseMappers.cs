using Firefly.Signal.Identity.Contracts.Responses;
using Firefly.Signal.Identity.Domain;
using Firefly.Signal.Identity.Infrastructure.Services;

namespace Firefly.Signal.Identity.Application.Mappers;

internal static class IdentityResponseMappers
{
    public static AuthenticatedUserResponse ToAuthenticatedUserResponse(UserAccount user)
        => new(user.Id, user.UserAccountName, user.DisplayName, user.Email, user.Role);

    public static LoginResponse ToLoginResponse(LoginTokenResult token, UserAccount user)
        => new(token.AccessToken, "Bearer", token.ExpiresAtUtc, ToAuthenticatedUserResponse(user));
}
