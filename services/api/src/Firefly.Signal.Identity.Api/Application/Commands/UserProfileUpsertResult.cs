using Firefly.Signal.Identity.Contracts.Responses;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed record UserProfileUpsertResult(UserProfileResponse Response, bool Created);
