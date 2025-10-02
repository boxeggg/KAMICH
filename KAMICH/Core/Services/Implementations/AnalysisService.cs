using KAMICH.Core.Models;
using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Services.Implementations;

public class AnalysisService : IAnalysisService
{
    private readonly IVehicleService _vehicleService;
    private readonly ISettingsService _settingsService;
    
    private Dictionary<Guid, double> _incomeCache = new Dictionary<Guid, double>();
    

    public AnalysisService(IVehicleService vehicleService, ISettingsService settingsService)
    {
        _vehicleService = vehicleService;
        _settingsService = settingsService;
    }
    
    public async Task<double> CalculateDailyIncome(List<MemoryVehicleDetails> vehicles)
    {
        if (vehicles.Count == 0) return 0;
        var settings = await _settingsService.LoadAsync();

        var localToday = DateTime.Now.Date;
        var from = new DateTimeOffset(localToday, DateTimeOffset.Now.Offset);
        var to = DateTimeOffset.Now;

        _incomeCache.Clear();

        foreach (var vehicle in vehicles)
        {
            var details = await _vehicleService.GetVehiclesDetails(vehicle.Id, from, to);

            var workHours = details.HoursBetweenFirstOnAndLastOff ?? 0;
            var mileage = details.DailyMileage ?? 0;

            var fuelPrice = vehicle.FuelPrice ?? settings.GlobalFuelPrice;
            var hourlyPrice = vehicle.HourlyPrice ?? settings.GlobalHourlyPrice;
            var operatorPrice = vehicle.OperatorPrice ?? settings.GlobalOperatorPrice;

            var income = (hourlyPrice * workHours)
                         - (operatorPrice * workHours)
                         - (fuelPrice * (mileage / 10.0));

            _incomeCache[vehicle.Id] = income;
        }

        return _incomeCache.Values.Sum();
    }




    public double GetIncomeForVehicle(Guid vehicleId)
    {
        if (_incomeCache.TryGetValue(vehicleId, out var income))
            return income;
        return 0;
    }
}