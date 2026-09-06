namespace KAMICH.Exceptions;

/// <summary>
/// Single entry point for handling errors: decides the policy, records the error
/// and - when the policy says so - shows the user a message.
/// </summary>
public interface IErrorHandler
{
    /// <summary>
    /// Handles an exception that was already caught.
    /// </summary>
    /// <param name="context">Where it happened, e.g. "FleetPage.LoadAsync".</param>
    /// <param name="policyOverride">Forces a policy instead of the one the exception carries.</param>
    /// <returns>
    /// The user-facing wording for this error, so a page can also show it inline
    /// (e.g. in an error label) instead of relying on an alert.
    /// </returns>
    Task<ErrorPresentation> HandleAsync(Exception exception, string? context = null, ErrorPolicy? policyOverride = null);

    /// <summary>Runs <paramref name="action"/> and handles anything it throws.</summary>
    /// <returns>
    /// <c>null</c> when it completed, otherwise the user-facing wording for the failure
    /// (so the caller can both detect it and show it inline).
    /// </returns>
    Task<ErrorPresentation?> SafeRunAsync(Func<Task> action, string? context = null, ErrorPolicy? policyOverride = null);

    /// <summary>Runs <paramref name="action"/> and handles anything it throws.</summary>
    /// <returns>The result, or <c>default</c> when it threw.</returns>
    Task<T?> SafeRunAsync<T>(Func<Task<T>> action, string? context = null, ErrorPolicy? policyOverride = null);
}
