using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Services.Implementations;

public interface ICacheService
{
    public Task<VehicleListModel?> GetCachedVehicles();
    public Task UpdateSingleCachedVehicle(VehicleModelDto model);
    public Task UpdateCachedVehicles(VehicleListModel? cachedVehicles);
}