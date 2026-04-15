using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;

namespace Firefly.Signal.Identity.Application.Queries;

public interface IAuthQueries
{
    Task<LoginResponse?> AuthenticateAsync(LoginUserRequest request, CancellationToken cancellationToken = default);
    Task<AuthenticatedUserResponse?> GetCurrentUserAsync(long userId, CancellationToken cancellationToken = default);
}
