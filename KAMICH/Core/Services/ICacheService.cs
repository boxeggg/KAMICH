using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Services;

public interface ICacheService
{
    Task<List<VehicleModelDto>?> GetCachedVehicles();
    Task UpdateCachedVehicles(List<VehicleModelDto>? vehicles);
}
