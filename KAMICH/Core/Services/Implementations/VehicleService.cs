using KAMICH.Integrations.Linqo.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace KAMICH.Core.Services.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly HttpClient _http;
        private const string ApiKey = "0S2qVxWCGkaqgVcVCnB_jKZQt44yRe3D";
        private DateTime _cacheTimeUtc;
        private readonly IMemoryService _memoryService;
        private readonly ICacheService _cacheService;

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

        public async Task<VehicleListModel?> GetVehicles(bool useCache = true, CancellationToken ct = default)
        {
            var vehicles = useCache ? (await _cacheService.GetCachedVehicles())?.Vehicles : null;

            if (vehicles == null)
            {
                var url = BuildUrl("objects", new Dictionary<string, string?>
                {
                    ["version"] = "1",
                    ["api_key"] = ApiKey
                });

                using var resp = await _http.GetAsync(url, ct);
                var text = await resp.Content.ReadAsStringAsync(ct);
                vehicles = JsonSerializer.Deserialize<List<VehicleModelDto>>(text, JsonOpts) ?? new List<VehicleModelDto>();
            }
            
            for (int i = 0; i < vehicles.Count; i++)
            {
                var memory = await _memoryService.GetMemoryVehiclesDetails(vehicles[i].Id);
                if (memory != null)
                {
                    vehicles[i].Color = memory.CustomColor;
                    vehicles[i].Icon = memory.CustomIcon;
                }
            }

            var vm = new VehicleListModel { Vehicles = vehicles };
            await _cacheService.UpdateCachedVehicles(vm);

            _cacheTimeUtc = DateTime.UtcNow;
            return vm;
        }

        public async Task<VehicleDetailsViewModel> GetVehiclesDetails(Guid objectId, DateTimeOffset from,
            DateTimeOffset to, CancellationToken ct = default)
        {
            var model = new PagedResult<EventDetails>
            {
                Events = new List<EventDetails>()
            };

            var url = BuildUrl("detected-events", new Dictionary<string, string?>
            {
                ["object_id"] = objectId.ToString(),
                ["from_datetime"] = from.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture),
                ["to_datetime"] = to.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture),
                ["limit"] = "100",
                ["version"] = "1",
                ["api_key"] = ApiKey
            });

            using var resp = await _http.GetAsync(url, ct);
            var text = await resp.Content.ReadAsStringAsync(ct);
            var paged = JsonSerializer.Deserialize<PagedResult<EventDetails>>(text, JsonOpts);
            var events = paged?.Events ?? new List<EventDetails>();
            var ignitionOnEvents = events
                .Where(e => e != null
                            && string.Equals(e.Name, "IgnitionOn", StringComparison.OrdinalIgnoreCase)
                            && e.Start?.Datetime != null
                            && e.Start.Datetime >= from
                            && e.Start.Datetime < to)
                .ToList();

            var firstOnStartDate = ignitionOnEvents
                .OrderBy(e => e.Start.Datetime)
                .FirstOrDefault()?.Start?.Datetime;
            var lastOnEndDate = ignitionOnEvents
                .OrderByDescending(e => e.Start.Datetime)
                .FirstOrDefault()?.End?.Datetime;
            var firstOnMileage = ignitionOnEvents
                .OrderBy(e => e.Start.Datetime)
                .FirstOrDefault()?.Start.Mileage;
            var lastOnMileage = ignitionOnEvents
                .Where(e => e.End != null)
                .OrderByDescending(e => e.End?.Datetime)
                .FirstOrDefault()?.End?.Mileage;

            double hours = 0.0;
            if (firstOnStartDate.HasValue && lastOnEndDate.HasValue)
            {
                var diff = lastOnEndDate.Value - firstOnStartDate.Value;
                if (diff.TotalSeconds > 0)
                    hours = diff.TotalHours;
            }

            double? dailyMileage = null;

            if (firstOnMileage.HasValue && lastOnMileage.HasValue)
            {
                var diff = lastOnMileage.Value - firstOnMileage.Value;
                if (diff >= 0)
                    dailyMileage = diff;
                else
                    dailyMileage = null;
            }

            var vehicleCache = await _cacheService.GetCachedVehicles() ?? new VehicleListModel();
            var cachedVehicle = vehicleCache.Vehicles.FirstOrDefault(v => v.Id == objectId);
            var vehicleMemory = await _memoryService.GetMemoryVehiclesDetails(objectId);

            var vm = new VehicleDetailsViewModel
            {
                Name = cachedVehicle?.Name ?? "",
                FirstIgnitionOn = ToPolandFromUtc(firstOnStartDate),
                LastIgnitionOff = ToPolandFromUtc(lastOnEndDate),
                HoursBetweenFirstOnAndLastOff = hours,
                DailyMileage = dailyMileage
            };
            if (vehicleMemory != null)
            {
                vm.CustomColor = vehicleMemory.CustomColor;
                vm.CustomIcon = vehicleMemory.CustomIcon;
            }

            return vm;
        }


        private static string BuildUrl(string path, IDictionary<string, string?> query)
        {
            var clean = path.Trim('/');
            var qs = string.Join('&', query
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));
            return string.IsNullOrEmpty(qs) ? clean : $"{clean}?{qs}";
        }

        private static readonly Lazy<TimeZoneInfo> PolandTz = new Lazy<TimeZoneInfo>(() =>
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw");
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                }
                catch
                {
                    return TimeZoneInfo.Local;
                }
            }
        });

        public static DateTimeOffset? ToPolandFromUtc(DateTime? utcDateTime)
        {
            if (!utcDateTime.HasValue) return null;

            var utc = DateTime.SpecifyKind(utcDateTime.Value, DateTimeKind.Utc);
            var dtoUtc = new DateTimeOffset(utc);

            return TimeZoneInfo.ConvertTime(dtoUtc, PolandTz.Value);
        }
    }
}