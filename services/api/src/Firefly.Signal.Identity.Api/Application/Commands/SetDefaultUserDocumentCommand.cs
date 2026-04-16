using Firefly.Signal.Identity.Contracts.Responses;
using MediatR;

namespace Firefly.Signal.Identity.Application.Commands;

public sealed record SetDefaultUserDocumentCommand(long UserId, long Id) : IRequest<UserDocumentResponse?>;
