using KAMICH.Core.Models;

namespace KAMICH.Core.Services;

public interface IIncomeHistoryService
{
    List<IncomeHistoryEntry> GetHistory(string period, int count);
    Task SaveEntry(IncomeHistoryEntry entry);
}
