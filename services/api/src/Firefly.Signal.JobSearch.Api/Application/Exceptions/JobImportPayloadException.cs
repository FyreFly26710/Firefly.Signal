using Firefly.Signal.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Firefly.Signal.JobSearch.Application.Exceptions;

public sealed class JobImportPayloadException : FireflyProblemException
{
    public JobImportPayloadException(string detail)
        : base(
            statusCode: StatusCodes.Status400BadRequest,
            title: "JSON import failed",
            detail: detail,
            errorCode: "job_import.invalid_payload")
    {
    }

    public JobImportPayloadException(string detail, Exception innerException)
        : base(
            statusCode: StatusCodes.Status400BadRequest,
            title: "JSON import failed",
            detail: detail,
            errorCode: "job_import.invalid_payload",
            innerException: innerException)
    {
    }
}
