using Firefly.Signal.Identity.Contracts.Responses;

namespace Firefly.Signal.Identity.Application.Queries;

public interface IUserProfileQueries
{
    Task<UserProfileResponse?> GetCurrentAsync(long userId, CancellationToken cancellationToken = default);
}
