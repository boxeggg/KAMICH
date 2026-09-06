namespace KAMICH.Exceptions;

/// <summary>
/// No API key is configured, so the request was never sent.
/// </summary>
public class MissingApiKeyException : AppException
{
    public MissingApiKeyException()
        : base("Brak skonfigurowanego klucza API.", ErrorPolicy.Notify)
    {
    }
}
