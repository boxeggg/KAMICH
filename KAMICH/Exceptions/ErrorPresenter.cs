using System.Net;
using System.Text.Json;

namespace KAMICH.Exceptions;

/// <summary>Title and body shown to the user for a given exception.</summary>
public sealed record ErrorPresentation(string Title, string Message);

public interface IErrorPresenter
{
    ErrorPresentation Present(Exception exception);
}

/// <summary>
/// Maps exceptions to Polish, user-facing wording. Technical detail stays in the log.
/// </summary>
public class ErrorPresenter : IErrorPresenter
{
    public ErrorPresentation Present(Exception exception) => exception switch
    {
        MissingApiKeyException => new(
            "Brak klucza API",
            "Skonfiguruj klucz API w ustawieniach, aby pobrać dane."),

        NetworkException { IsTimeout: true } => new(
            "Przekroczono czas oczekiwania",
            "Serwer nie odpowiedział na czas. Spróbuj ponownie za chwilę."),

        NetworkException => new(
            "Brak połączenia",
            "Nie udało się połączyć z serwerem. Sprawdź połączenie z internetem i spróbuj ponownie."),

        ApiException apiEx => PresentApi(apiEx),

        OperationCanceledException => new(
            "Anulowano",
            "Operacja została przerwana."),

        HttpRequestException => new(
            "Brak połączenia",
            "Nie udało się połączyć z serwerem. Sprawdź połączenie z internetem i spróbuj ponownie."),

        JsonException => new(
            "Nieprawidłowa odpowiedź",
            "Serwer zwrócił dane w nieoczekiwanym formacie. Spróbuj ponownie później."),

        _ => new(
            "Nieoczekiwany błąd",
            string.IsNullOrWhiteSpace(exception.Message)
                ? "Wystąpił nieoczekiwany błąd."
                : exception.Message)
    };

    private static ErrorPresentation PresentApi(ApiException ex)
    {
        var status = (int)ex.StatusCode;

        return ex.StatusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => new(
                "Nieprawidłowy klucz API",
                "Serwer odrzucił klucz API. Sprawdź go w ustawieniach."),

            HttpStatusCode.NotFound => new(
                "Nie znaleziono danych",
                "Serwer nie znalazł żądanych danych."),

            HttpStatusCode.BadRequest or HttpStatusCode.UnprocessableEntity => new(
                "Nieprawidłowe żądanie",
                ServerMessageOr(ex, "Serwer odrzucił żądanie jako nieprawidłowe.")),

            HttpStatusCode.TooManyRequests => new(
                "Zbyt wiele żądań",
                "Serwer chwilowo ogranicza liczbę zapytań. Spróbuj ponownie za chwilę."),

            _ when status >= 500 => new(
                "Błąd serwera",
                $"Serwer zwrócił błąd (kod {status}). Spróbuj ponownie później."),

            _ => new(
                "Błąd",
                ServerMessageOr(ex, $"Serwer zwrócił nieoczekiwaną odpowiedź (kod {status})."))
        };
    }

    private static string ServerMessageOr(ApiException ex, string fallback) =>
        ex.Error?.HasMessage == true ? ex.Error.Message! : fallback;
}
