using KAMICH.Integrations.Linqo.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KAMICH.Integrations.Linqo
{
    public sealed class TelemetryResponse
    {
        [JsonPropertyName("items")]
        public List<TelemetryPoint> Items { get; set; } = new();

        [JsonPropertyName("continuation_token")]
        public string? ContinuationToken { get; set; }
    }

    public sealed class TelemetryPoint
    {
        [JsonPropertyName("object_id")] public Guid ObjectId { get; set; }
        [JsonPropertyName("datetime")] public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("ignition_status")]
        public IgnitionStatus IgnitionStatus { get; set; }

        [JsonPropertyName("position")] public Geo? Position { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum IgnitionStatus { OFF, ON }

    public sealed class Geo
    {
        [JsonPropertyName("latitude")] public double Lat { get; set; }
        [JsonPropertyName("longitude")] public double Lng { get; set; }
    }
}