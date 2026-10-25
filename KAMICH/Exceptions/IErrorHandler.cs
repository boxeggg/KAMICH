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

    /// <summary>
    /// Runs <paramref name="action"/> and handles anything it throws, so callers
    /// need no try/catch of their own. Never throws.
    /// </summary>
    /// <returns>
    /// <c>null</c> when it completed, otherwise the user-facing wording for the failure
    /// (so the caller can both detect it and show it inline).
    /// </returns>
    Task<ErrorPresentation?> RunAsync(Func<Task> action, string? context = null, ErrorPolicy? policyOverride = null);
}
