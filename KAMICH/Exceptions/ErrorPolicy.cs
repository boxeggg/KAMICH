namespace KAMICH.Exceptions;

/// <summary>
/// What the app does when an error reaches <see cref="IErrorHandler"/>.
/// </summary>
public enum ErrorPolicy
{
    /// <summary>Swallow silently (e.g. cancelled operations).</summary>
    Ignore = 0,

    /// <summary>Write to <see cref="IErrorLog"/> only, no UI.</summary>
    Log = 1,

    /// <summary>Log and show an alert. Reserved for cases the user can act on.</summary>
    Notify = 2
}
