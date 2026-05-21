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
    
    public async Task<double> CalculateDailyIncome(CancellationToken cts, List<MemoryVehicleDetails> vehicles)
    {
        if (vehicles.Count == 0) return 0;
        var settings = await _settingsService.LoadAsync();

        var localToday = DateTime.Now.Date;
        var from = new DateTimeOffset(localToday, DateTimeOffset.Now.Offset);
        var to = DateTimeOffset.Now;

        _incomeCache.Clear();

        foreach (var vehicle in vehicles)
        {
            var details = await _vehicleService.GetVehiclesDetails(vehicle.Id, from, to, ct: cts);

            var workHours = details.HoursBetweenFirstOnAndLastOff ?? 0;
            var mileage = details.DailyMileage ?? 0;

            var fuelPrice = vehicle.FuelPrice ?? settings.GlobalVehicleSettings.FuelPrice;
            var hourlyPrice = vehicle.HourlyPrice ?? settings.GlobalVehicleSettings.HourlyPrice;
            var operatorPrice = vehicle.OperatorPrice ?? settings.GlobalVehicleSettings.OperatorPrice;
            var fuelConsumptionPerHour = vehicle.FuelConsumptionPerHour ?? settings.GlobalVehicleSettings.FuelConsumptionPerHour;

            var income = (hourlyPrice * workHours)
                       - (operatorPrice * workHours)
                       - (fuelConsumptionPerHour * workHours * fuelPrice);

            _incomeCache[vehicle.Id] = income;
        }

        return _incomeCache.Values.Sum();
    }




    public double GetIncomeForVehicle(CancellationToken cts, Guid vehicleId)
    {
        if (_incomeCache.TryGetValue(vehicleId, out var income))
            return income;
        return 0;
    }

    public async Task<double> CalculateIncomeFromStats(CancellationToken cts, List<MemoryVehicleDetails> vehicles, string statsType)
    {
        if (vehicles.Count == 0) return 0;
        var settings = await _settingsService.LoadAsync();

        _incomeCache.Clear();

        foreach (var vehicle in vehicles)
        {
            var stats = await _vehicleService.GetStats(vehicle.Id, statsType, ct: cts);
            var latest = stats.FirstOrDefault();
            if (latest == null) continue;

            var workHours = latest.TotalHours ?? 0;

            var fuelPrice = vehicle.FuelPrice ?? settings.GlobalVehicleSettings.FuelPrice;
            var hourlyPrice = vehicle.HourlyPrice ?? settings.GlobalVehicleSettings.HourlyPrice;
            var operatorPrice = vehicle.OperatorPrice ?? settings.GlobalVehicleSettings.OperatorPrice;
            var fuelConsumptionPerHour = vehicle.FuelConsumptionPerHour ?? settings.GlobalVehicleSettings.FuelConsumptionPerHour;

            var income = (hourlyPrice * workHours)
                       - (operatorPrice * workHours)
                       - (fuelConsumptionPerHour * workHours * fuelPrice);

            _incomeCache[vehicle.Id] = income;
        }

        return _incomeCache.Values.Sum();
    }

    public async Task<double> CalculateIncomeFromWorkLogs(CancellationToken cts, List<MemoryVehicleDetails> vehicles, DateTime date)
    {
        if (vehicles.Count == 0) return 0;
        var settings = await _settingsService.LoadAsync();

        _incomeCache.Clear();

        foreach (var vehicle in vehicles)
        {
            var workLog = await _vehicleService.GetWorkLog(vehicle.Id, date, ct: cts);
            var workHours = workLog?.HoursWorked ?? 0;

            var fuelPrice = vehicle.FuelPrice ?? settings.GlobalVehicleSettings.FuelPrice;
            var hourlyPrice = vehicle.HourlyPrice ?? settings.GlobalVehicleSettings.HourlyPrice;
            var operatorPrice = vehicle.OperatorPrice ?? settings.GlobalVehicleSettings.OperatorPrice;
            var fuelConsumptionPerHour = vehicle.FuelConsumptionPerHour ?? settings.GlobalVehicleSettings.FuelConsumptionPerHour;

            var income = (hourlyPrice * workHours)
                       - (operatorPrice * workHours)
                       - (fuelConsumptionPerHour * workHours * fuelPrice);

            _incomeCache[vehicle.Id] = income;
        }

        return _incomeCache.Values.Sum();
    }

    public async Task<double> CalculateIncomeFromStatsByPeriod(CancellationToken cts, List<MemoryVehicleDetails> vehicles, string statsType, DateTime periodStart)
    {
        if (vehicles.Count == 0) return 0;
        var settings = await _settingsService.LoadAsync();

        _incomeCache.Clear();

        foreach (var vehicle in vehicles)
        {
            var stats = await _vehicleService.GetStatsByPeriod(vehicle.Id, statsType, periodStart, ct: cts);
            var workHours = stats?.TotalHours ?? 0;

            var fuelPrice = vehicle.FuelPrice ?? settings.GlobalVehicleSettings.FuelPrice;
            var hourlyPrice = vehicle.HourlyPrice ?? settings.GlobalVehicleSettings.HourlyPrice;
            var operatorPrice = vehicle.OperatorPrice ?? settings.GlobalVehicleSettings.OperatorPrice;
            var fuelConsumptionPerHour = vehicle.FuelConsumptionPerHour ?? settings.GlobalVehicleSettings.FuelConsumptionPerHour;

            var income = (hourlyPrice * workHours)
                       - (operatorPrice * workHours)
                       - (fuelConsumptionPerHour * workHours * fuelPrice);

            _incomeCache[vehicle.Id] = income;
        }

        return _incomeCache.Values.Sum();
    }
}