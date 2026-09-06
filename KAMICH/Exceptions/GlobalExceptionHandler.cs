namespace KAMICH.Exceptions;

/// <summary>
/// Last-resort net for exceptions no page caught. These are logged only: the app may
/// already be tearing down, and there is nothing actionable to tell the user.
/// </summary>
public static class GlobalExceptionHandler
{
    private static bool _installed;

    public static void Install(IErrorHandler handler)
    {
        if (_installed) return;
        _installed = true;

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                _ = handler.HandleAsync(ex, "AppDomain.UnhandledException", ErrorPolicy.Log);
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            args.SetObserved();
            _ = handler.HandleAsync(args.Exception, "TaskScheduler.UnobservedTaskException", ErrorPolicy.Log);
        };
    }
}
