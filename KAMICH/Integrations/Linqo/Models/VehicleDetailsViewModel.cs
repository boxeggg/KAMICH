using System.Text.Json.Serialization;

namespace KAMICH.Integrations.Linqo.Models;

public class VehicleDetailsViewModel
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("firstIgnitionOn")] public DateTimeOffset? FirstIgnitionOn { get; set; }
    [JsonPropertyName("lastIgnitionOff")] public DateTimeOffset? LastIgnitionOff { get; set; }
    [JsonPropertyName("hoursBetweenFirstOnAndLastOff")] public double? HoursBetweenFirstOnAndLastOff { get; set; }
    [JsonPropertyName("dailyMileage")] public double? DailyMileage { get; set; }

    [JsonIgnore] public string FirstIgnitionOnPoland => FirstIgnitionOn?.ToString("HH:mm");
    [JsonIgnore] public string LastIgnitionOffPoland => LastIgnitionOff?.ToString("HH:mm");

    [JsonIgnore] public Color? CustomColor { get; set; }
    [JsonIgnore] public string? CustomIcon { get; set; }
}
