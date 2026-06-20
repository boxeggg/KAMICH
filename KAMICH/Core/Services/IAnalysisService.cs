using KAMICH.Core.Models;

namespace KAMICH.Core.Services;

public interface IAnalysisService
{
    Task<double> CalculateDailyIncome(CancellationToken cts, List<MemoryVehicleDetails> vehicles);
    double GetIncomeForVehicle(CancellationToken cts, Guid vehicleId);
    double GetTotalHours();
    double GetHoursForVehicle(Guid vehicleId);
    Task<double> CalculateIncomeFromStats(CancellationToken cts, List<MemoryVehicleDetails> vehicles, string statsType);
    Task<double> CalculateIncomeFromWorkLogs(CancellationToken cts, List<MemoryVehicleDetails> vehicles, DateTime date);
    Task<double> CalculateIncomeFromStatsByPeriod(CancellationToken cts, List<MemoryVehicleDetails> vehicles, string statsType, DateTime periodStart);
}