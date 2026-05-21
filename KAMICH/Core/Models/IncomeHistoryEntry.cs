namespace KAMICH.Core.Models;

public class IncomeHistoryEntry
{
    public DateTime Date { get; set; }
    public string Period { get; set; } // DAILY, MONTHLY, YEARLY
    public double Income { get; set; }
}
