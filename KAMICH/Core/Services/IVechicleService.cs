using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Services;

public interface IVehicleService
{
    Task<List<VehicleModelDto>> GetVehicles(CancellationToken ct = default);
    Task<VehicleDetailsViewModel> GetVehiclesDetails(Guid objectId, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
}
