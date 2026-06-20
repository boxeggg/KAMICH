using System.Text.Json.Serialization;

namespace KAMICH.Integrations.Linqo.Models;

public class VehicleDetailsDto
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("firstIgnitionOn")] public DateTimeOffset? FirstIgnitionOn { get; set; }
    [JsonPropertyName("lastIgnitionOff")] public DateTimeOffset? LastIgnitionOff { get; set; }
    [JsonPropertyName("hoursBetweenFirstOnAndLastOff")] public double? HoursBetweenFirstOnAndLastOff { get; set; }
    [JsonPropertyName("dailyMileage")] public double? DailyMileage { get; set; }
    [JsonPropertyName("isCurrentlyWorking")] public bool isCurrentlyWorking { get; set; }
}
