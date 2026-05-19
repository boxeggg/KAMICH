using KAMICH.Core.Services;

namespace KAMICH.Pages;

public partial class LandingPage : ContentPage
{
    private readonly IHomeService _homeService;
    private string _currentPeriod = "DAILY";

    public LandingPage(IHomeService homeService)
    {
        InitializeComponent();
        _homeService = homeService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        UpdateTabStyles();
        await LoadData();
    }

    private async void OnDailyClicked(object sender, EventArgs e)
    {
        if (_currentPeriod == "DAILY") return;
        _currentPeriod = "DAILY";
        UpdateTabStyles();
        await LoadData();
    }

    private async void OnMonthlyClicked(object sender, EventArgs e)
    {
        if (_currentPeriod == "MONTHLY") return;
        _currentPeriod = "MONTHLY";
        UpdateTabStyles();
        await LoadData();
    }

    private async void OnYearlyClicked(object sender, EventArgs e)
    {
        if (_currentPeriod == "YEARLY") return;
        _currentPeriod = "YEARLY";
        UpdateTabStyles();
        await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;

            var vm = await _homeService.GetHomePageViewModel(_currentPeriod);
            BindingContext = vm;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", ex.Message, "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private void UpdateTabStyles()
    {
        var activeBackground = Color.FromArgb("#2E7D32");
        var activeLightBackground = Color.FromArgb("#E8F5E9");

        bool isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        var inactiveBackground = Colors.Transparent;
        var activeText = isDark ? Colors.White : Colors.White;
        var inactiveText = isDark ? Color.FromArgb("#9E9E9E") : Color.FromArgb("#757575");
        var activeBg = isDark ? activeBackground : activeBackground;

        TabDaily.BackgroundColor = _currentPeriod == "DAILY" ? activeBg : inactiveBackground;
        TabMonthly.BackgroundColor = _currentPeriod == "MONTHLY" ? activeBg : inactiveBackground;
        TabYearly.BackgroundColor = _currentPeriod == "YEARLY" ? activeBg : inactiveBackground;

        TabDailyLabel.TextColor = _currentPeriod == "DAILY" ? activeText : inactiveText;
        TabMonthlyLabel.TextColor = _currentPeriod == "MONTHLY" ? activeText : inactiveText;
        TabYearlyLabel.TextColor = _currentPeriod == "YEARLY" ? activeText : inactiveText;
    }
}
