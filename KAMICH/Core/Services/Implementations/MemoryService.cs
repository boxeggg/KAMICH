using KAMICH.Core.Models;
using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Services.Implementations;

public class MemoryService : IMemoryService
{
    private readonly List<MemoryVehicleDetails> _vehicles = new List<MemoryVehicleDetails>();

    public List<MemoryVehicleDetails> GetMemoryVehicles()
    {
        return _vehicles;
    }

    public Task<MemoryVehicleDetails?> GetMemoryVehiclesDetails(Guid objectId)
    {
        var vehicle = _vehicles.FirstOrDefault(x => x.Id == objectId);
        return Task.FromResult(vehicle);
    }

    public Task<bool> SetMemoryVehicle(MemoryVehicleDetails vehicleDetailsViewModel)
    {
        _vehicles.Add(vehicleDetailsViewModel);
        return Task.FromResult(true);
    }
}