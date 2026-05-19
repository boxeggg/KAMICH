namespace KAMICH.Core.Models;

public class HomePageViewModel
{
    public string PeriodLabel { get; set; } = "Przychód dzienny";
    public double TotalIncome { get; set; }
    public IEnumerable<SimpleVehicleVm> Vehicles { get; set; } = new List<SimpleVehicleVm>();
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
