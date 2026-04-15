using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed record UserProfileUpsertResult(UserProfileResponse Response, bool Created);

public interface IUserProfileCommands
{
    Task<UserProfileUpsertResult?> UpsertCurrentAsync(long userId, UserProfileRequest request, CancellationToken cancellationToken = default);
}
