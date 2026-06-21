using KAMICH.Core.Models;

namespace KAMICH.Core.ViewModels;

public class HomePageViewModel
{
    public string PeriodLabel { get; set; } = "Przychód dzienny";
    public double TotalIncome { get; set; }
    public double TotalHours { get; set; }
    public IEnumerable<SimpleVehicleVm> Vehicles { get; set; } = new List<SimpleVehicleVm>();
    public double FixedCostHotel { get; set; }
    public double FixedCostTransport { get; set; }
    public double FixedCostService { get; set; }
    public double FixedCostOther { get; set; }
    public double FixedCostTotal => FixedCostHotel + FixedCostTransport + FixedCostService + FixedCostOther;
    public bool HasFixedCosts => FixedCostTotal > 0;
    public double NetIncome => TotalIncome - FixedCostTotal;
    public IReadOnlyList<ChartPoint> ChartPoints { get; set; } = new List<ChartPoint>(); // Char points should be independent from extrenal libs
    public bool HasChartData => ChartPoints.Count > 0;
}

public class SimpleVehicleVm
{
    public string Name { get; set; }
    public Guid Id { get; set; }
    public double Income { get; set; }
    public double Hours { get; set; }
    public Color Color { get; set; }
    public string Icon { get; set; }
    public bool IsCurrentlyWorking { get; set; }
}
