using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.ViewModels;

public class StatsViewModel
{
    public string Type { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public double? TotalHours { get; set; }
    public double? TotalMileage { get; set; }

    public string PeriodLabel => Type switch
    {
        "WEEKLY" => $"{PeriodStart:dd.MM} - {PeriodEnd:dd.MM.yyyy}",
        "MONTHLY" => PeriodStart.ToString("MMMM yyyy"),
        "YEARLY" => PeriodStart.ToString("yyyy"),
        "ALLTIME" => "Wszystkie dane",
        _ => $"{PeriodStart:dd.MM.yyyy}"
    };

    public static StatsViewModel FromDto(StatsDto dto) => new()
    {
        Type = dto.Type,
        PeriodStart = dto.PeriodStart,
        PeriodEnd = dto.PeriodEnd,
        TotalHours = dto.TotalHours,
        TotalMileage = dto.TotalMileage
    };
}
