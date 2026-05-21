using Microcharts;

namespace KAMICH.Core.Models;

public class HomePageViewModel
{
    public string PeriodLabel { get; set; } = "Przychód dzienny";
    public double TotalIncome { get; set; }
    public IEnumerable<SimpleVehicleVm> Vehicles { get; set; } = new List<SimpleVehicleVm>();
    public double FixedCostHotel { get; set; }
    public double FixedCostTransport { get; set; }
    public double FixedCostService { get; set; }
    public double FixedCostOther { get; set; }
    public double FixedCostTotal => FixedCostHotel + FixedCostTransport + FixedCostService + FixedCostOther;
    public bool HasFixedCosts => FixedCostTotal > 0;
    public double NetIncome => TotalIncome - FixedCostTotal;
    public List<ChartEntry> ChartEntries { get; set; } = new();
    public bool HasChartData => ChartEntries.Count > 0;
}

public class SimpleVehicleVm
{
    public string Name { get; set; }
    public Guid Id { get; set; }
    public double Income { get; set; }
    public double Hours { get; set; }
    public Color Color { get; set; }
    public string Icon { get; set; }
}
