using KAMICH.Core.Models;
using KAMICH.Integrations.Linqo.Models;

namespace KAMICH.Core.Services;

public interface IMemoryService
{
   public List<MemoryVehicleDetails> GetMemoryVehicles();
   public Task<MemoryVehicleDetails?> GetMemoryVehiclesDetails(Guid objectId);
   public Task<bool> SetMemoryVehicle(MemoryVehicleDetails vehicleDetailsViewModel);
}