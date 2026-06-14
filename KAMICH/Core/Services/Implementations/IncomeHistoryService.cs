using KAMICH.Core.Models;
using System.Text.Json;

namespace KAMICH.Core.Services.Implementations;

public class IncomeHistoryService : IIncomeHistoryService
{
    private readonly List<IncomeHistoryEntry> _entries = new();
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public IncomeHistoryService()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "income_history.json");
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var list = JsonSerializer.Deserialize<List<IncomeHistoryEntry>>(json);
                if (list != null)
                    _entries.AddRange(list);
            }
        }
        catch { }
    }

    public List<IncomeHistoryEntry> GetHistory(string period, int count)
    {
        return _entries
            .Where(e => e.Period == period)
            .OrderByDescending(e => e.Date)
            .Take(count)
            .OrderBy(e => e.Date)
            .ToList();
    }

    public async Task SaveEntry(IncomeHistoryEntry entry)
    {
        await _lock.WaitAsync();
        try
        {
            var existing = _entries.FindIndex(e => e.Period == entry.Period && e.Date == entry.Date);
            if (existing >= 0)
                _entries[existing] = entry;
            else
                _entries.Add(entry);

            var json = JsonSerializer.Serialize(_entries);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _lock.Release();
        }
    }
}
