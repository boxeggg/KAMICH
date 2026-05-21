using KAMICH.Core.Services.Implementations;

namespace KAMICH.Pages;

public partial class SetupPage : ContentPage
{
    private readonly ISettingsService _settingsService;

    public SetupPage(ISettingsService settingsService)
    {
        _settingsService = settingsService;
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

        try
        {
            await _settingsService.SetApiKeyAsync(apiKey);
            await Shell.Current.GoToAsync("//home");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", $"Nie udało się zapisać konfiguracji.\n{ex.Message}", "OK");
            StartButton.IsEnabled = true;
            StartButton.Text = "Rozpocznij";
        }
    }
}
