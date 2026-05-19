using System.Text.Json.Serialization;

namespace KAMICH.Integrations.Linqo.Models;

public class StatsViewModel
{
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("periodStart")] public DateTime PeriodStart { get; set; }
    [JsonPropertyName("periodEnd")] public DateTime PeriodEnd { get; set; }
    [JsonPropertyName("totalHours")] public double? TotalHours { get; set; }
    [JsonPropertyName("totalMileage")] public double? TotalMileage { get; set; }
    [JsonPropertyName("calculatedAt")] public DateTime? CalculatedAt { get; set; }

    [JsonIgnore]
    public string PeriodLabel => Type switch
    {
        "WEEKLY" => $"{PeriodStart:dd.MM} - {PeriodEnd:dd.MM.yyyy}",
        "MONTHLY" => PeriodStart.ToString("MMMM yyyy"),
        "YEARLY" => PeriodStart.ToString("yyyy"),
        "ALLTIME" => "Wszystkie dane",
        _ => $"{PeriodStart:dd.MM.yyyy}"
    };
}
