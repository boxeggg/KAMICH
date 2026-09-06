using System.Net;
using System.Net.Sockets;

namespace KAMICH.Exceptions;

public class ErrorHandler : IErrorHandler
{
    private readonly IErrorLog _log;
    private readonly IErrorPresenter _presenter;

    // Prevents a burst of parallel failures from stacking alerts on top of each other.
    private readonly SemaphoreSlim _alertGate = new(1, 1);
    private bool _alertVisible;

    public ErrorHandler(IErrorLog log, IErrorPresenter presenter)
    {
        _log = log;
        _presenter = presenter;
    }

    public async Task<ErrorPresentation> HandleAsync(Exception exception, string? context = null, ErrorPolicy? policyOverride = null)
    {
        // Unwrap the common single-inner aggregate so the policy is picked from the real cause.
        if (exception is AggregateException aggregate && aggregate.InnerExceptions.Count == 1)
            exception = aggregate.InnerExceptions[0];

        var presentation = _presenter.Present(exception);

        // A cancelled operation is never an error, whatever policy the caller asked for.
        if (exception is OperationCanceledException) return presentation;

        var policy = policyOverride ?? ResolvePolicy(exception);
        if (policy == ErrorPolicy.Ignore) return presentation;

        await _log.WriteAsync(BuildEntry(exception, context, policy));

        if (policy == ErrorPolicy.Notify)
            await NotifyAsync(presentation);

        return presentation;
    }

    public async Task<ErrorPresentation?> SafeRunAsync(Func<Task> action, string? context = null, ErrorPolicy? policyOverride = null)
    {
        try
        {
            await action();
            return null;
        }
        catch (Exception ex)
        {
            return await HandleAsync(ex, context, policyOverride);
        }
    }

    public async Task<T?> SafeRunAsync<T>(Func<Task<T>> action, string? context = null, ErrorPolicy? policyOverride = null)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            await HandleAsync(ex, context, policyOverride);
            return default;
        }
    }

    /// <summary>
    /// Exceptions we raise ourselves carry their policy. For the rest: connectivity
    /// failures are actionable, and anything unexpected is worth showing.
    /// </summary>
    private static ErrorPolicy ResolvePolicy(Exception exception) => exception switch
    {
        AppException appEx => appEx.Policy,
        HttpRequestException or WebException or SocketException or TimeoutException => ErrorPolicy.Notify,
        _ => ErrorPolicy.Notify
    };

    private static ErrorLogEntry BuildEntry(Exception exception, string? context, ErrorPolicy policy) => new()
    {
        Context = context,
        ExceptionType = exception.GetType().Name,
        Message = exception.Message,
        Details = exception.ToString(),
        Policy = policy,
        StatusCode = (exception as ApiException)?.StatusCode,
        RequestPath = (exception as ApiException)?.RequestPath
    };

    private async Task NotifyAsync(ErrorPresentation presentation)
    {
        await _alertGate.WaitAsync();
        try
        {
            if (_alertVisible) return;
            _alertVisible = true;
        }
        finally
        {
            _alertGate.Release();
        }

        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var page = CurrentPage;
                if (page is null) return;
                await page.DisplayAlert(presentation.Title, presentation.Message, "OK");
            });
        }
        catch (Exception ex)
        {
            // The alert itself failed (no window, page torn down) - keep the original error logged.
            await _log.WriteAsync(BuildEntry(ex, "ErrorHandler.NotifyAsync", ErrorPolicy.Log));
        }
        finally
        {
            _alertVisible = false;
        }
    }

    private static Page? CurrentPage =>
        Shell.Current?.CurrentPage
        ?? Application.Current?.Windows.FirstOrDefault()?.Page;
}
