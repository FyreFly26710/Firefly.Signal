using Firefly.Signal.Identity.Application.Commands;
using Firefly.Signal.Identity.Contracts.Requests;

namespace Firefly.Signal.Identity.Api.Apis;

internal static class UserApiMappers
{
    public static CreateUserCommand ToCreateCommand(CreateUserRequest request)
        => new(
            UserAccount: request.UserAccount,
            Password: request.Password,
            Email: request.Email,
            DisplayName: request.DisplayName,
            Role: request.Role);

    public static UpdateUserCommand ToUpdateCommand(long id, UpdateUserRequest request)
        => new(
            Id: id,
            Email: request.Email,
            DisplayName: request.DisplayName,
            Role: request.Role);

    public static DeleteUserCommand ToDeleteCommand(long id)
        => new(Id: id);
}
