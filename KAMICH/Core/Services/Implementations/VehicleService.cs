using KAMICH.Integrations.Linqo.Models;
using System.Globalization;
using System.Text.Json;

namespace KAMICH.Core.Services.Implementations;

public class VehicleService : IVehicleService
{
    private readonly HttpClient _http;
    private readonly IMemoryService _memoryService;
    private readonly ICacheService _cacheService;
    private const string ApiKey = "0S2qVxWCGkaqgVcVCnB_jKZQt44yRe3D";

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public VehicleService(HttpClient http, IMemoryService memoryService, ICacheService cacheService)
    {
        _http = http;
        _memoryService = memoryService;
        _cacheService = cacheService;
    }

    public async Task<List<VehicleModelDto>> GetVehicles(CancellationToken ct = default, bool bypassCache = false)
    {
        if (!bypassCache)
        {
            var cached = await _cacheService.GetCachedVehicles();
            if (cached != null)
                return cached;
        }

        using var req = new HttpRequestMessage(HttpMethod.Get, "api/vehicles");
        req.Headers.Add("X-Api-Key", ApiKey);

        using var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync(ct);
        var vehicles = JsonSerializer.Deserialize<List<VehicleModelDto>>(text, JsonOpts) ?? new();

        foreach (var v in vehicles)
        {
            var memory = await _memoryService.GetMemoryVehiclesDetails(v.Id);
            if (memory != null)
            {
                v.Color = memory.CustomColor;
                v.Icon = memory.CustomIcon;
            }
        }

        await _cacheService.UpdateCachedVehicles(vehicles);
        return vehicles;
    }

    public async Task<VehicleDetailsViewModel> GetVehiclesDetails(Guid objectId, DateTimeOffset from,
        DateTimeOffset to, CancellationToken ct = default)
    {
        var fromStr = from.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ssZ", CultureInfo.InvariantCulture);
        var toStr = to.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ssZ", CultureInfo.InvariantCulture);

        using var req = new HttpRequestMessage(HttpMethod.Get,
            $"api/vehicles/{objectId}/details?from={Uri.EscapeDataString(fromStr)}&to={Uri.EscapeDataString(toStr)}");
        req.Headers.Add("X-Api-Key", ApiKey);

        using var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync(ct);
        var vm = JsonSerializer.Deserialize<VehicleDetailsViewModel>(text, JsonOpts) ?? new();

        var vehicleMemory = await _memoryService.GetMemoryVehiclesDetails(objectId);
        if (vehicleMemory != null)
        {
            vm.CustomColor = vehicleMemory.CustomColor;
            vm.CustomIcon = vehicleMemory.CustomIcon;
        }

        return vm;
    }

    public async Task<List<StatsViewModel>> GetStats(Guid vehicleId, string type, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get,
            $"api/vehicles/{vehicleId}/stats/{type}/history");
        req.Headers.Add("X-Api-Key", ApiKey);

        using var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<List<StatsViewModel>>(text, JsonOpts) ?? new();
    }
}
