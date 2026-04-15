using Firefly.Signal.Identity.Contracts.Requests;
using Firefly.Signal.Identity.Contracts.Responses;

namespace Firefly.Signal.Identity.Application.Commands;

public interface IUserCommands
{
    Task<AuthenticatedUserResponse?> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<AuthenticatedUserResponse?> UpdateAsync(long id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
