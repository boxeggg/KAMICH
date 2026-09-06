namespace KAMICH.Exceptions;

/// <summary>
/// Sink for recorded errors. In-memory today; swap the DI registration for an
/// API-backed implementation to persist them server-side.
/// </summary>
public interface IErrorLog
{
    Task WriteAsync(ErrorLogEntry entry, CancellationToken ct = default);

    /// <summary>Most recent entries first.</summary>
    IReadOnlyList<ErrorLogEntry> GetRecent(int max = 100);

    void Clear();
}
