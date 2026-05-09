using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Services.Implementations;

public class CacheService : ICacheService
{
    private List<VehicleModelDto>? _cachedVehicles;

    public Task<List<VehicleModelDto>?> GetCachedVehicles()
    {
        return Task.FromResult(_cachedVehicles);
    }

    public Task UpdateCachedVehicles(List<VehicleModelDto>? vehicles)
    {
        _cachedVehicles = vehicles;
        return Task.CompletedTask;
    }
}
