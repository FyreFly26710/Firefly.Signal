using MediatR;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed record DeleteUserCommand(long Id) : IRequest<bool>;
