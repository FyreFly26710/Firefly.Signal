using Firefly.Signal.Identity.Contracts.Responses;
using MediatR;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed record CreateUserCommand(
    string UserAccount,
    string Password,
    string? Email,
    string? DisplayName,
    string Role) : IRequest<AuthenticatedUserResponse?>;
