using Firefly.Signal.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Firefly.Signal.JobSearch.Application.Exceptions;

public sealed class InvalidApplicationStatusTransitionException(string detail)
    : FireflyProblemException(
        statusCode: StatusCodes.Status400BadRequest,
        title: "Invalid status transition",
        detail: detail,
        errorCode: "job_application.invalid_status_transition");
