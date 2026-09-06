using System.Net;

namespace KAMICH.Exceptions;

/// <summary>
/// One recorded error. Kept provider-agnostic so the sink can later be swapped
/// for an API-backed one without touching call sites.
/// </summary>
public sealed class ErrorLogEntry
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

    /// <summary>Where the error came from, e.g. "LandingPage.LoadData".</summary>
    public string? Context { get; init; }

    public required string ExceptionType { get; init; }

    public required string Message { get; init; }

    /// <summary>Full exception text, including inner exceptions and stack trace.</summary>
    public string? Details { get; init; }

    public ErrorPolicy Policy { get; init; }

    public HttpStatusCode? StatusCode { get; init; }

    public string? RequestPath { get; init; }

    public override string ToString()
    {
        var status = StatusCode.HasValue ? $" [{(int)StatusCode.Value}]" : string.Empty;
        var context = string.IsNullOrWhiteSpace(Context) ? string.Empty : $" ({Context})";
        return $"{Timestamp:yyyy-MM-dd HH:mm:ss} {ExceptionType}{status}{context}: {Message}";
    }
}
