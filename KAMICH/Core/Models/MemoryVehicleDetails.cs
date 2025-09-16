using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Models;

public class MemoryVehicleDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double? FuelPrice { get; set; }
    public double? HourlyPrice { get; set; }
    public double? OperatorPrice { get; set; }
    public Color? CustomColor { get; set; }
    public string? CustomIcon { get; set; }
}