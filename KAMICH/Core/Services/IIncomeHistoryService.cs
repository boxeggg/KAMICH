using KAMICH.Core.Models;

namespace KAMICH.Core.Services;

public interface IIncomeHistoryService
{
    List<IncomeHistoryEntry> GetHistory(string period, int count = 12);
    Task SaveEntry(IncomeHistoryEntry entry);
}
