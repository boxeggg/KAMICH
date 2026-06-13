using System.Text.Json.Serialization;

namespace KAMICH.Integrations.Linqo.Models;

public class StatsDto
{
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("periodStart")] public DateTime PeriodStart { get; set; }
    [JsonPropertyName("periodEnd")] public DateTime PeriodEnd { get; set; }
    [JsonPropertyName("totalHours")] public double? TotalHours { get; set; }
    [JsonPropertyName("totalMileage")] public double? TotalMileage { get; set; }
    [JsonPropertyName("calculatedAt")] public DateTime? CalculatedAt { get; set; }
}
