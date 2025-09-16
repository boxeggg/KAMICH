using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KAMICH.Integrations.Linqo.Models
{
    public class PagedResult<T>
    {
        [JsonPropertyName("events")]
        public List<T> Events { get; set; }

        [JsonPropertyName("continuation_token")]
        public string ContinuationToken { get; set; }
    }

    public class EventDetails
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("driver_id")]
        public string DriverId { get; set; }

        [JsonPropertyName("trip_type")]
        public string TripType { get; set; }

        [JsonPropertyName("start")]
        public EventPoint Start { get; set; }

        [JsonPropertyName("end")]
        public EventPoint? End { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }
    }

    public class EventPoint
    {
        [JsonPropertyName("datetime")]
        public DateTime? Datetime { get; set; }

        [JsonPropertyName("location")]
        public EventLocation Location { get; set; }

        [JsonPropertyName("mileage")]
        public double? Mileage { get; set; }

        [JsonPropertyName("speed")]
        public double? Speed { get; set; }
    }

    public class EventLocation
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
