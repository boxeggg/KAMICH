using KAMICH.Core.Models;
using KAMICH.Integrations.Linqo.Models;

using System.Text.Json;
using System.IO;
using Microsoft.Maui.Storage;
using System.Threading;

namespace KAMICH.Core.Services.Implementations;

public class MemoryService : IMemoryService
{
    private readonly List<MemoryVehicleDetails> _vehicles = new List<MemoryVehicleDetails>();
    private readonly string _filePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly ICacheService _cacheService;

    public MemoryService(ICacheService cacheService)
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "vehicles.json");
        _cacheService = cacheService;
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var list = JsonSerializer.Deserialize<List<MemoryVehicleDetails>>(json);
                if (list != null)
                    _vehicles.AddRange(list);
            }
        }
        catch
        {
            throw new FileNotFoundException(_filePath);
        }
    }

    public List<MemoryVehicleDetails> GetMemoryVehicles()
    {
        return _vehicles;
    }

    public Task<MemoryVehicleDetails?> GetMemoryVehiclesDetails(Guid objectId)
    {
        var vehicle = _vehicles.FirstOrDefault(x => x.Id == objectId);
        return Task.FromResult(vehicle);
    }
    
    public async Task<bool> SetMemoryVehicle(MemoryVehicleDetails vehicleDetailsViewModel)
    {
        try
        {
            await _fileLock.WaitAsync();
            
            var existingIndex = _vehicles.FindIndex(v => v.Id == vehicleDetailsViewModel.Id);

            if (existingIndex == -1)
            {
                _vehicles.Add(vehicleDetailsViewModel);
            }
            else
            {
                _vehicles[existingIndex] = vehicleDetailsViewModel;
            }

            var json = JsonSerializer.Serialize(_vehicles);
            await File.WriteAllTextAsync(_filePath, json);

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _fileLock.Release();
        }
    }

}
