using Firefly.Signal.Identity.Contracts.Responses;
using MediatR;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed record UpdateUserCommand(long Id, string? Email, string? DisplayName, string? Role) : IRequest<AuthenticatedUserResponse?>;
