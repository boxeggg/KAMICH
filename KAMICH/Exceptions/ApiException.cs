using System.Net;

namespace KAMICH.Exceptions;

/// <summary>
/// The API answered with a non-success status code.
/// </summary>
public class ApiException : AppException
{
    private const int RawBodyPreviewLength = 500;

    public ApiException(
        HttpStatusCode statusCode,
        ApiError? error,
        string? rawBody,
        string? requestPath,
        Exception? innerException = null)
        : base(BuildMessage(statusCode, error, rawBody, requestPath), PolicyFor(statusCode), innerException)
    {
        StatusCode = statusCode;
        Error = error;
        RawBody = rawBody;
        RequestPath = requestPath;
    }

    public HttpStatusCode StatusCode { get; }

    /// <summary>Parsed error body, when the API returned one in the expected shape.</summary>
    public ApiError? Error { get; }

    /// <summary>Unparsed response body, kept for the log.</summary>
    public string? RawBody { get; }

    public string? RequestPath { get; }

    public bool IsAuthError =>
        StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;

    /// <summary>
    /// API errors are logged by default; only the ones the user can fix raise an alert.
    /// </summary>
    private static ErrorPolicy PolicyFor(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => ErrorPolicy.Notify,
        _ => ErrorPolicy.Log
    };

    private static string BuildMessage(HttpStatusCode statusCode, ApiError? error, string? rawBody, string? requestPath)
    {
        var detail = error?.HasMessage == true
            ? error.Message
            : Truncate(rawBody);

        var path = string.IsNullOrWhiteSpace(requestPath) ? "(nieznana ścieżka)" : requestPath;
        return string.IsNullOrWhiteSpace(detail)
            ? $"API {(int)statusCode} {statusCode} @ {path}"
            : $"API {(int)statusCode} {statusCode} @ {path}: {detail}";
    }

    private static string? Truncate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        value = value.Trim();
        return value.Length <= RawBodyPreviewLength
            ? value
            : value[..RawBodyPreviewLength] + "…";
    }
}
