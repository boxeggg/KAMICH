using System.Globalization;
using KAMICH.Core.Services.Implementations;
using KAMICH.Exceptions;

namespace KAMICH
{
    public partial class App : Application
    {
        private readonly ISettingsService _settingsService;
        private readonly IErrorHandler _errors;

        public App(ISettingsService settingsService, IErrorHandler errors)
        {
            var polish = new CultureInfo("pl-PL");
            CultureInfo.DefaultThreadCurrentCulture = polish;
            CultureInfo.DefaultThreadCurrentUICulture = polish;

            InitializeComponent();
            _settingsService = settingsService;
            _errors = errors;
            var dark = Preferences.Get("settings.dark_mode", false);
            this.UserAppTheme = dark ? AppTheme.Dark : AppTheme.Light;
            _ = LoadSettingsInBackground();

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
#if WINDOWS || MACCATALYST
            window.Width = 1100;
            window.Height = 760;
            window.MinimumWidth = 800;
            window.MinimumHeight = 600;
#endif
            return window;
        }
        // Startup work with no UI yet: failures are recorded, never shown.
        private Task LoadSettingsInBackground() => _errors.SafeRunAsync(async () =>
        {
            if (_settingsService is null) return;

            var model = await _settingsService.LoadAsync();

            if (model.DarkMode != Preferences.Get("settings.dark_mode", false))
            {
                Preferences.Set("settings.dark_mode", model.DarkMode);
            }

            if (model.DarkMode)
            {
                MainThread.BeginInvokeOnMainThread(() => this.UserAppTheme = AppTheme.Dark);
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => this.UserAppTheme = AppTheme.Light);
            }
        }, "App.LoadSettingsInBackground", ErrorPolicy.Log);
    }
}