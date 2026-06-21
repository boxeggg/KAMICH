using KAMICH.Core.Models;
using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Services.Implementations;

public class AnalysisService : IAnalysisService
{
    private readonly IVehicleService _vehicleService;
    private readonly ISettingsService _settingsService;

    // One entry per vehicle, holding the result of the most recent Calculate* call.
    private readonly Dictionary<Guid, VehicleAnalysis> _cache = new();

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

        _cache.Clear();

        foreach (var vehicle in vehicles)
        {
            var details = await _vehicleService.GetVehiclesDetails(vehicle.Id, from, to, ct: cts);
            var workHours = details.HoursBetweenFirstOnAndLastOff ?? 0;

            _cache[vehicle.Id] = new VehicleAnalysis(
                CalculateIncome(vehicle, settings, workHours),
                workHours,
                details.isCurrentlyWorking);
        }

        return _cache.Values.Sum(a => a.Income);
    }

    public async Task<double> CalculateIncomeFromStats(CancellationToken cts, List<MemoryVehicleDetails> vehicles, string statsType)
    {
        if (vehicles.Count == 0) return 0;
        var settings = await _settingsService.LoadAsync();

        _cache.Clear();

        foreach (var vehicle in vehicles)
        {
            var stats = await _vehicleService.GetStats(vehicle.Id, statsType, ct: cts);
            var latest = stats.FirstOrDefault();
            if (latest == null) continue;

            var workHours = latest.TotalHours ?? 0;
            _cache[vehicle.Id] = new VehicleAnalysis(CalculateIncome(vehicle, settings, workHours), workHours, false);
        }

        return _cache.Values.Sum(a => a.Income);
    }

    public async Task<double> CalculateIncomeFromWorkLogs(CancellationToken cts, List<MemoryVehicleDetails> vehicles, DateTime date)
    {
        if (vehicles.Count == 0) return 0;
        var settings = await _settingsService.LoadAsync();

        _cache.Clear();

        foreach (var vehicle in vehicles)
        {
            var workLog = await _vehicleService.GetWorkLog(vehicle.Id, date, ct: cts);
            var workHours = workLog?.HoursWorked ?? 0;
            _cache[vehicle.Id] = new VehicleAnalysis(CalculateIncome(vehicle, settings, workHours), workHours, false);
        }

        return _cache.Values.Sum(a => a.Income);
    }

    public async Task<double> CalculateIncomeFromStatsByPeriod(CancellationToken cts, List<MemoryVehicleDetails> vehicles, string statsType, DateTime periodStart)
    {
        if (vehicles.Count == 0) return 0;
        var settings = await _settingsService.LoadAsync();

        _cache.Clear();

        foreach (var vehicle in vehicles)
        {
            var stats = await _vehicleService.GetStatsByPeriod(vehicle.Id, statsType, periodStart, ct: cts);
            var workHours = stats?.TotalHours ?? 0;
            _cache[vehicle.Id] = new VehicleAnalysis(CalculateIncome(vehicle, settings, workHours), workHours, false);
        }

        return _cache.Values.Sum(a => a.Income);
    }

    public double GetIncomeForVehicle(CancellationToken cts, Guid vehicleId) =>
        _cache.TryGetValue(vehicleId, out var a) ? a.Income : 0;

    public double GetHoursForVehicle(Guid vehicleId) =>
        _cache.TryGetValue(vehicleId, out var a) ? a.Hours : 0;

    public bool IsVehicleWorking(Guid vehicleId) =>
        _cache.TryGetValue(vehicleId, out var a) && a.IsWorking;

    public double GetTotalHours() => _cache.Values.Sum(a => a.Hours);

    private static double CalculateIncome(MemoryVehicleDetails vehicle, AppSettingsModel settings, double workHours)
    {
        var fuelPrice = vehicle.FuelPrice ?? settings.GlobalVehicleSettings.FuelPrice;
        var hourlyPrice = vehicle.HourlyPrice ?? settings.GlobalVehicleSettings.HourlyPrice;
        var operatorPrice = vehicle.OperatorPrice ?? settings.GlobalVehicleSettings.OperatorPrice;
        var fuelConsumptionPerHour = vehicle.FuelConsumptionPerHour ?? settings.GlobalVehicleSettings.FuelConsumptionPerHour;

        return (hourlyPrice * workHours)
             - (operatorPrice * workHours)
             - (fuelConsumptionPerHour * workHours * fuelPrice);
    }

    // Per-vehicle analysis result for the current period.
    private sealed record VehicleAnalysis(double Income, double Hours, bool IsWorking);
}
