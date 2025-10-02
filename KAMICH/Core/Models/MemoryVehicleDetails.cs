using System.Text.Json;
using System.Text.Json.Serialization;
using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Models;

public class MemoryVehicleDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsTracked { get; set; } = false;
    public double? FuelPrice { get; set; }
    public double? HourlyPrice { get; set; }
    public double? OperatorPrice { get; set; }
    public string CustomColorHex { get; set; } = "#808080";
    public string CustomIcon { get; set; } = "&#xF1B9;";
    [JsonIgnore]
    public Color CustomColor
    {
        get => ColorFromHex(CustomColorHex);
        set => CustomColorHex = ToHex(value);
    }

    private string ToHex(Color value)
    {
        int r = (int)Math.Round(value.Red * 255);
        int g = (int)Math.Round(value.Green * 255);
        int b = (int)Math.Round(value.Blue * 255);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private Color ColorFromHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            return Colors.Gray; 

        hex = hex.Trim().TrimStart('#');

        if (hex.Length == 6)
        {
            var r = Convert.ToByte(hex.Substring(0, 2), 16);
            var g = Convert.ToByte(hex.Substring(2, 2), 16);
            var b = Convert.ToByte(hex.Substring(4, 2), 16);
            return Color.FromRgb(r, g, b);
        }
        else if (hex.Length == 8) // AARRGGBB
        {
            var a = Convert.ToByte(hex.Substring(0, 2), 16);
            var r = Convert.ToByte(hex.Substring(2, 2), 16);
            var g = Convert.ToByte(hex.Substring(4, 2), 16);
            var b = Convert.ToByte(hex.Substring(6, 2), 16);
            return Color.FromRgba(r / 255.0, g / 255.0, b / 255.0, a / 255.0);
        }

        return Colors.Gray; // fallback
    }
}
