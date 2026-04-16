using MediatR;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed record DeleteUserDocumentCommand(long UserId, long Id) : IRequest<bool>;
