using System.Diagnostics;

namespace KAMICH.Exceptions;

/// <summary>
/// Bounded in-memory sink. Also mirrors entries to the debug output so failures
/// stay visible while debugging.
/// </summary>
public class InMemoryErrorLog : IErrorLog
{
    private const int Capacity = 200;

    private readonly LinkedList<ErrorLogEntry> _entries = new();
    private readonly object _gate = new();

    public Task WriteAsync(ErrorLogEntry entry, CancellationToken ct = default)
    {
        lock (_gate)
        {
            _entries.AddFirst(entry);
            while (_entries.Count > Capacity)
                _entries.RemoveLast();
        }

        Debug.WriteLine($"[ERROR] {entry}");
        if (!string.IsNullOrWhiteSpace(entry.Details))
            Debug.WriteLine(entry.Details);

        return Task.CompletedTask;
    }

    public IReadOnlyList<ErrorLogEntry> GetRecent(int max = 100)
    {
        lock (_gate)
        {
            return _entries.Take(max).ToList();
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _entries.Clear();
        }
    }
}
