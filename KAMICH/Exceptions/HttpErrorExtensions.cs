using System.Text.Json;

namespace KAMICH.Exceptions;

/// <summary>
/// Integration point between HttpClient and the typed exceptions: transport failures
/// become <see cref="NetworkException"/>, non-success responses become
/// <see cref="ApiException"/> carrying the parsed error body.
/// Use instead of <c>SendAsync</c> + <c>EnsureSuccessStatusCode</c>.
/// </summary>
public static class HttpErrorExtensions
{
    private static readonly JsonSerializerOptions ErrorJsonOpts = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public static Task<HttpResponseMessage> SendApiAsync(
        this HttpClient http,
        HttpRequestMessage request,
        CancellationToken ct = default) =>
        http.SendApiAsync(request, HttpCompletionOption.ResponseContentRead, ct);

    public static async Task<HttpResponseMessage> SendApiAsync(
        this HttpClient http,
        HttpRequestMessage request,
        HttpCompletionOption completionOption,
        CancellationToken ct = default)
    {
        try
        {
            return await http.SendAsync(request, completionOption, ct);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            // A TaskCanceledException without a cancelled token is the HttpClient timeout.
            throw new NetworkException(
                $"Przekroczono czas oczekiwania na odpowiedź: {request.RequestUri}", isTimeout: true, ex);
        }
        catch (HttpRequestException ex)
        {
            throw new NetworkException(
                $"Nie udało się połączyć z serwerem: {request.RequestUri}", isTimeout: false, ex);
        }
    }

    /// <summary>
    /// Throws <see cref="ApiException"/> when the response is not a success status.
    /// Reads the error body first so the exception (and the log) carries the reason
    /// the server gave.
    /// </summary>
    public static async Task EnsureApiSuccessAsync(this HttpResponseMessage response, CancellationToken ct = default)
    {
        if (response.IsSuccessStatusCode) return;

        string? body = null;
        try
        {
            body = await response.Content.ReadAsStringAsync(ct);
        }
        catch
        {
            // A body we cannot read must not hide the status code.
        }

        throw new ApiException(
            response.StatusCode,
            TryParseError(body),
            body,
            response.RequestMessage?.RequestUri?.PathAndQuery);
    }

    private static ApiError? TryParseError(string? body)
    {
        if (string.IsNullOrWhiteSpace(body)) return null;

        var trimmed = body.TrimStart();
        if (trimmed.Length == 0 || (trimmed[0] != '{' && trimmed[0] != '[')) return null;

        try
        {
            return JsonSerializer.Deserialize<ApiError>(trimmed, ErrorJsonOpts);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
