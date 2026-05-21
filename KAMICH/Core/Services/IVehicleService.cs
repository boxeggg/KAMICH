using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Services;

public interface IVehicleService
{
    Task<List<VehicleModelDto>> GetVehicles(CancellationToken ct = default, bool bypassCache = false);
    Task<VehicleDetailsViewModel> GetVehiclesDetails(Guid objectId, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
    Task<List<StatsViewModel>> GetStats(Guid vehicleId, string type, CancellationToken ct = default);
    Task<StatsViewModel?> GetStatsByPeriod(Guid vehicleId, string type, DateTime periodStart, CancellationToken ct = default);
    Task<WorkLogDto?> GetWorkLog(Guid vehicleId, DateTime date, CancellationToken ct = default);
}
