namespace KAMICH.Exceptions;

/// <summary>
/// The request never produced a response: no connectivity, DNS/TLS failure or timeout.
/// Notifies by default because the user can act on it (check the connection, retry).
/// </summary>
public class NetworkException : AppException
{
    public NetworkException(string message, bool isTimeout, Exception? innerException = null)
        : base(message, ErrorPolicy.Notify, innerException)
    {
        IsTimeout = isTimeout;
    }

    public bool IsTimeout { get; }
}
