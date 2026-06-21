
using System.Globalization;
using KAMICH.Core.Models;
using KAMICH.Core.Services.Implementations;

namespace KAMICH.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly ISettingsService _settings;
    private bool _loading;

    public SettingsPage(ISettingsService settings)
    {
        InitializeComponent();
        _settings = settings;

        VersionLabel.Text = $"Wersja {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        _loading = true;
        var m = await _settings.LoadAsync();
        DarkModeSwitch.IsToggled = m.DarkMode;
        ApiKeyEntry.Text = m.ApiKey;
        ApiKeyEntry.IsPassword = true;
        ApiKeyToggleBtn.Text = "Pokaż";
        FuelPriceEntry.Text = m.GlobalVehicleSettings.FuelPrice.ToString();
        OperatorPriceEntry.Text = m.GlobalVehicleSettings.OperatorPrice.ToString();
        HourlyPriceEntry.Text = m.GlobalVehicleSettings.HourlyPrice.ToString();
        FuelConsumptionEntry.Text = m.GlobalVehicleSettings.FuelConsumptionPerHour.ToString();
        FixedHotelEntry.Text = m.MonthlyFixedCosts.Hotel.ToString();
        FixedTransportEntry.Text = m.MonthlyFixedCosts.Transport.ToString();
        FixedServiceEntry.Text = m.MonthlyFixedCosts.Service.ToString();
        FixedOtherEntry.Text = m.MonthlyFixedCosts.Other.ToString();

        _loading = false;
    }

    private async Task SaveNonSecretsAsync()
        => await _settings.SaveAsync(new AppSettingsModel
        {
            DarkMode = DarkModeSwitch.IsToggled,
            GlobalVehicleSettings = new VehicleStatsModel
            {
                FuelPrice = double.TryParse((FuelPriceEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var f) ? f : 0.0,
                OperatorPrice = double.TryParse((OperatorPriceEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var d) ? d : 0.0,
                HourlyPrice = double.TryParse((HourlyPriceEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var e) ? e : 0.0,
                FuelConsumptionPerHour = double.TryParse((FuelConsumptionEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var g) ? g : 0.0,
            },
            MonthlyFixedCosts = new FixedCostsModel
            {
                Hotel = double.TryParse((FixedHotelEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number, CultureInfo.InvariantCulture, out var h) ? h : 0.0,
                Transport = double.TryParse((FixedTransportEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number, CultureInfo.InvariantCulture, out var t) ? t : 0.0,
                Service = double.TryParse((FixedServiceEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number, CultureInfo.InvariantCulture, out var s) ? s : 0.0,
                Other = double.TryParse((FixedOtherEntry?.Text ?? "").Trim().Replace(',', '.'),
                NumberStyles.Number, CultureInfo.InvariantCulture, out var o) ? o : 0.0,
            },
            ApiKey = ApiKeyEntry.Text
        });

    private async void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        if (_loading) return;
        Preferences.Set("settings.dark_mode", e.Value);
        var theme = e.Value ? AppTheme.Dark : AppTheme.Light;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current.UserAppTheme = theme;
        });
        await SaveNonSecretsAsync();
    }
    private async void OnParameterChange(object? sender, FocusEventArgs focusEventArgs)
    {
        if (_loading) return;
        await SaveNonSecretsAsync();
    }

    
    private async void OnApiKeyCompleted(object? sender, EventArgs e)
    {
        if (_loading) return;
        await _settings.SetApiKeyAsync(ApiKeyEntry.Text);
    }

    private async void OnApiKeyUnfocused(object? sender, FocusEventArgs e)
    {
        if (_loading) return;
        await _settings.SetApiKeyAsync(ApiKeyEntry.Text);
    }

    private void OnToggleApiKeyVisibility(object? sender, EventArgs e)
    {
        ApiKeyEntry.IsPassword = !ApiKeyEntry.IsPassword;
        ApiKeyToggleBtn.Text = ApiKeyEntry.IsPassword ? "Pokaż" : "Ukryj";
    }

    private async void OnClearNonSecretsTapped(object? sender, TappedEventArgs e)
    {
        _settings.ResetNonSecrets();
        await LoadAsync();
    }
    private async void OnHelpTapped(object sender, EventArgs e)
    {
        const string title = "Pomoc — Parametry systemowe";
        const string message =
            "Te wartości są ustawieniami domyślnymi dla całego systemu.\n\n" +
            "• Cena paliwa – podstawowa cena za litr, używana do obliczeń.\n" +
            "• Stawka godzinowa – domyślna stawka za godzinę pracy.\n" +
            "• Stawka operatora – dodatkowa stawka przypisana operatorowi.\n\n" +
            "• Pobór paliwa – Pobór paliwa za godzine pracy (Możliwość personalizacji trybu pracy w ustawieniach koparki).\n\n" +
            "Uwaga: Te wartości można później nadpisać indywidualnie dla każdego pojazdu, " +
            "jeżeli będzie potrzeba użycia innych stawek.";

        await DisplayAlert(title, message, "OK");
    }
}