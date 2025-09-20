using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KAMICH.Integrations.Linqo.Models
{
    public class VehicleModelDto
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonIgnore] 
        public Color Color { get; set; } = Color.FromRgb(128, 128, 128);

        [JsonIgnore]
        public string Icon { get; set; } = "\uf1b9";

        
    }
}
