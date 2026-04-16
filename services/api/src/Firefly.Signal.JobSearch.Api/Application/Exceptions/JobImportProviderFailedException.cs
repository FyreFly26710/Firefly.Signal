using Firefly.Signal.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Firefly.Signal.JobSearch.Application.Exceptions;

public sealed class JobImportProviderFailedException(string detail, Exception? innerException = null)
    : FireflyProblemException(
        statusCode: StatusCodes.Status400BadRequest,
        title: "Provider import failed",
        detail: detail,
        errorCode: "job_import.provider_failed",
        innerException: innerException);
