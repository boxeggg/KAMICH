namespace KAMICH.Core.Models;

public class HomePageViewModel
{
    public double DailyIncome { get; set; }
    public double DailyHours { get; set; }
    public IEnumerable<SimpleVehicleVm> Vehicles { get; set; }
}

public class SimpleVehicleVm
{
    public string Name { get; set; }
    public Guid Id { get; set; }
    public double Income { get; set; }
    
    public double Hours {get; set;}
    public Color Color { get; set; }
    public string Icon { get; set; }
}