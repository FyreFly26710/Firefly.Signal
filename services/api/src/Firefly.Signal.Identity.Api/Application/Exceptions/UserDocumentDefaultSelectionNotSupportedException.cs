using Firefly.Signal.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Firefly.Signal.Identity.Application.Exceptions;

public sealed class UserDocumentDefaultSelectionNotSupportedException()
    : FireflyProblemException(
        statusCode: StatusCodes.Status400BadRequest,
        title: "Default selection not supported",
        detail: "Only CV and cover-letter documents can be marked as default.",
        errorCode: "user_document.default_not_supported");
