namespace Firefly.Signal.SharedKernel.Exceptions;

public class FireflyProblemException : Exception
{
    public FireflyProblemException(
        int statusCode,
        string title,
        string detail,
        string? errorCode = null,
        Exception? innerException = null)
        : base(detail, innerException)
    {
        StatusCode = statusCode;
        Title = title;
        Detail = detail;
        ErrorCode = errorCode;
    }

    public int StatusCode { get; }

    public string Title { get; }

    public string Detail { get; }

    public string? ErrorCode { get; }
}
