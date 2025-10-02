using KAMICH.Core.Models;

namespace KAMICH.Core.Services;

public interface IAnalysisService
{
    Task<double> CalculateDailyIncome(List<MemoryVehicleDetails> vehicles);
    double GetIncomeForVehicle(Guid vehicleId);
}