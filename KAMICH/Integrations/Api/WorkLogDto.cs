using System.Text.Json.Serialization;

namespace KAMICH.Integrations.Api;

public class WorkLogDto
{
    [JsonPropertyName("date")] public DateTime Date { get; set; }
    [JsonPropertyName("hoursWorked")] public double? HoursWorked { get; set; }
    [JsonPropertyName("mileage")] public double? Mileage { get; set; }
}
