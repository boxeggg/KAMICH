using KAMICH.Core.Services.Implementations;
using KAMICH.Exceptions;

namespace KAMICH.Pages;

public partial class SetupPage : ContentPage
{
    private readonly ISettingsService _settingsService;
    private readonly IErrorHandler _errors;

    public SetupPage(ISettingsService settingsService, IErrorHandler errors)
    {
        _settingsService = settingsService;
        _errors = errors;
        InitializeComponent();
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        var apiKey = ApiKeyEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            ApiKeyError.Text = "Klucz API jest wymagany.";
            ApiKeyError.IsVisible = true;
            return;
        }

        if (!EventsConfirmCheckbox.IsChecked)
        {
            await DisplayAlert("Uwaga", "Potwierdź konfigurację zdarzeń IgnitionOn i IgnitionOff.", "OK");
            return;
        }

        ApiKeyError.IsVisible = false;
        StartButton.IsEnabled = false;
        StartButton.Text = "Konfigurowanie...";

        var error = await _errors.SafeRunAsync(async () =>
        {
            await _settingsService.SetApiKeyAsync(apiKey);
            await Shell.Current.GoToAsync("//home");
        }, "SetupPage.OnStartClicked", ErrorPolicy.Notify);

        if (error is not null)
        {
            StartButton.IsEnabled = true;
            StartButton.Text = "Rozpocznij";
        }
    }
}
