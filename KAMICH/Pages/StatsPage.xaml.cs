using KAMICH.Core.Services;
using KAMICH.Core.ViewModels;
using KAMICH.Exceptions;

namespace KAMICH.Pages;

[QueryProperty(nameof(VehicleId), "VehicleId")]
public partial class StatsPage : ContentPage
{
    private Guid _vehicleId;
    private readonly IVehicleService _vehicleService;
    private readonly IErrorHandler _errors;
    private string _currentType = "WEEKLY";

    public string VehicleId
    {
        get => _vehicleId.ToString();
        set
        {
            if (Guid.TryParse(value, out var parsed))
                _vehicleId = parsed;
        }
    }

    public StatsPage(IVehicleService vehicleService, IErrorHandler errors)
    {
        _vehicleService = vehicleService;
        _errors = errors;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        UpdateTabStyles();
        await LoadStats();
    }

    private async void OnWeeklyClicked(object sender, EventArgs e)
    {
        _currentType = "WEEKLY";
        UpdateTabStyles();
        await LoadStats();
    }

    private async void OnMonthlyClicked(object sender, EventArgs e)
    {
        _currentType = "MONTHLY";
        UpdateTabStyles();
        await LoadStats();
    }

    private async void OnYearlyClicked(object sender, EventArgs e)
    {
        _currentType = "YEARLY";
        UpdateTabStyles();
        await LoadStats();
    }

    private async Task LoadStats()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        EmptyLabel.IsVisible = false;
        StatsCollection.ItemsSource = null;

        // No inline error surface on this page, so the user gets an alert.
        await _errors.SafeRunAsync(async () =>
        {
            var stats = await _vehicleService.GetStats(_vehicleId, _currentType);

            if (stats == null || stats.Count == 0)
            {
                EmptyLabel.IsVisible = true;
            }
            else
            {
                StatsCollection.ItemsSource = stats.Select(StatsViewModel.FromDto).ToList();
            }
        }, "StatsPage.LoadStats", ErrorPolicy.Notify);

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
    }

    private void UpdateTabStyles()
    {
        var activeColor = Color.FromArgb("#2E7D32");
        var inactiveColor = Color.FromArgb("#808080");

        BtnWeekly.BackgroundColor = _currentType == "WEEKLY" ? activeColor : inactiveColor;
        BtnMonthly.BackgroundColor = _currentType == "MONTHLY" ? activeColor : inactiveColor;
        BtnYearly.BackgroundColor = _currentType == "YEARLY" ? activeColor : inactiveColor;
    }
}
