namespace KAMICH.Exceptions;

/// <summary>
/// Base for exceptions the app raises deliberately. Carries the default
/// <see cref="ErrorPolicy"/> for that kind of failure; callers can still override it.
/// </summary>
public abstract class AppException : Exception
{
    protected AppException(string message, ErrorPolicy policy, Exception? innerException = null)
        : base(message, innerException)
    {
        Policy = policy;
    }

    public ErrorPolicy Policy { get; }
}
