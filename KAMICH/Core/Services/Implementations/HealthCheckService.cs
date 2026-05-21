using KAMICH.Integrations.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KAMICH.Core.Services.Implementations
{
    internal class HealthCheckService : IHealthCheckService
    {
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
        private readonly HttpClient _http;
        public HealthCheckService(HttpClient http) { 
            _http = http;
        }
        public async Task<bool> IsApiHealthy()
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                using var req = new HttpRequestMessage(HttpMethod.Get, "actuator/health");
                var result = await _http.SendAsync(req, cts.Token);
                var text = await result.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<HealthCheckDto>(text, JsonOpts);
                return dto?.Status == "UP";
            }
            catch
            {
                return false;
            }
        }
    }
}
