using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Services.Implementations;

public class CacheService : ICacheService
{
    private VehicleListModel? _cachedVehicles;
    public Task<VehicleListModel?> GetCachedVehicles()
    {
        return Task.FromResult(_cachedVehicles);
    }

    public Task UpdateSingleCachedVehicle(VehicleModelDto model)
    {
        var objectToUpdate = _cachedVehicles.Vehicles.FirstOrDefault(x => x.Id == model.Id);
        objectToUpdate.Name = model.Name;
        objectToUpdate.Color = model.Color;
        objectToUpdate.Icon = model.Icon;
        return Task.CompletedTask;
    }

    public Task UpdateCachedVehicles(VehicleListModel? cachedVehicles)
    {
       _cachedVehicles = cachedVehicles;
       return Task.CompletedTask;
    }
}