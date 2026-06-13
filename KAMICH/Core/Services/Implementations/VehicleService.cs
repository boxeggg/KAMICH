using KAMICH.Integrations.Linqo.Models;
using System.Globalization;
using System.Text.Json;

namespace KAMICH.Core.Services.Implementations;

public class VehicleService : IVehicleService
{
    private readonly HttpClient _http;
    private readonly ICacheService _cacheService;
    private readonly ISettingsService _settings;

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public VehicleService(HttpClient http, ICacheService cacheService, ISettingsService settings)
    {
        _http = http;
        _cacheService = cacheService;
        _settings = settings;
    }

    private async Task<string> GetApiKeyOrThrow()
    {
        var key = await _settings.GetApiKeyAsync();
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("Brak klucza API. Skonfiguruj go w ustawieniach.");
        return key;
    }

    public async Task<List<VehicleModelDto>> GetVehicles(CancellationToken ct = default, bool bypassCache = false)
    {
        if (!bypassCache)
        {
            var cached = await _cacheService.GetCachedVehicles();
            if (cached != null)
                return cached;
        }

        var apiKey = await GetApiKeyOrThrow();
        using var req = new HttpRequestMessage(HttpMethod.Get, "api/vehicles");
        req.Headers.Add("X-Api-Key", apiKey);

        using var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync(ct);
        var vehicles = JsonSerializer.Deserialize<List<VehicleModelDto>>(text, JsonOpts) ?? new();

        await _cacheService.UpdateCachedVehicles(vehicles);
        return vehicles;
    }

    public async Task<VehicleDetailsDto> GetVehiclesDetails(Guid objectId, DateTimeOffset from,
        DateTimeOffset to, CancellationToken ct = default)
    {
        var fromStr = from.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ssZ", CultureInfo.InvariantCulture);
        var toStr = to.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ssZ", CultureInfo.InvariantCulture);

        var apiKey = await GetApiKeyOrThrow();
        using var req = new HttpRequestMessage(HttpMethod.Get,
            $"api/vehicles/{objectId}/details?from={Uri.EscapeDataString(fromStr)}&to={Uri.EscapeDataString(toStr)}");
        req.Headers.Add("X-Api-Key", apiKey);

        using var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<VehicleDetailsDto>(text, JsonOpts) ?? new();
    }

    public async Task<List<StatsDto>> GetStats(Guid vehicleId, string type, CancellationToken ct = default)
    {
        var apiKey = await GetApiKeyOrThrow();
        using var req = new HttpRequestMessage(HttpMethod.Get,
            $"api/vehicles/{vehicleId}/stats/{type}/history");
        req.Headers.Add("X-Api-Key", apiKey);

        using var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<List<StatsDto>>(text, JsonOpts) ?? new();
    }

    public async Task<StatsDto?> GetStatsByPeriod(Guid vehicleId, string type, DateTime periodStart, CancellationToken ct = default)
    {
        var apiKey = await GetApiKeyOrThrow();
        var dateStr = periodStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        using var req = new HttpRequestMessage(HttpMethod.Get,
            $"api/vehicles/{vehicleId}/stats/{type}?periodStart={dateStr}");
        req.Headers.Add("X-Api-Key", apiKey);

        using var resp = await _http.SendAsync(req, ct);
        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(text)) return null;
        return JsonSerializer.Deserialize<StatsDto>(text, JsonOpts);
    }

    public async Task<WorkLogDto?> GetWorkLog(Guid vehicleId, DateTime date, CancellationToken ct = default)
    {
        var apiKey = await GetApiKeyOrThrow();
        var dateStr = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        using var req = new HttpRequestMessage(HttpMethod.Get,
            $"api/vehicles/{vehicleId}/worklog?date={dateStr}");
        req.Headers.Add("X-Api-Key", apiKey);

        using var resp = await _http.SendAsync(req, ct);
        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(text)) return null;
        return JsonSerializer.Deserialize<WorkLogDto>(text, JsonOpts);
    }
}
