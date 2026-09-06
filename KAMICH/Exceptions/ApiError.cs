using System.Text.Json.Serialization;

namespace KAMICH.Exceptions;

/// <summary>
/// Error body returned by the KAMICH API. Shaped after the Spring Boot default
/// error payload; every field is optional because error bodies are not guaranteed.
/// </summary>
public class ApiError
{
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }

    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>Application-specific error code, when the API sends one.</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    public bool HasMessage => !string.IsNullOrWhiteSpace(Message);
}
