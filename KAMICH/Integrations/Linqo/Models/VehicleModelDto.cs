using System.Text.Json.Serialization;

namespace KAMICH.Integrations.Linqo.Models;

public class VehicleModelDto
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}
