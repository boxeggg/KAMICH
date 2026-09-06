using KAMICH.Core.Services;
using KAMICH.Core.Services.Implementations;
using KAMICH.Core.Models;
using KAMICH.Exceptions;
using Microcharts;
using SkiaSharp;
using KAMICH.Core.ViewModels;
#if ANDROID
using KAMICH.Platforms.Android;
#endif
namespace KAMICH.Pages;

public partial class LandingPage : ContentPage
{
    private readonly IHomeService _homeService;
    private readonly ISettingsService _settings;
    private readonly IApkUpdateService _updateService;
    private readonly IErrorHandler _errors;
    private string _currentPeriod = "DAILY";
    private CancellationTokenSource _cts;
    private bool _doneHealthCheck = false;
    private bool _useBarChart = true;
#if WINDOWS || MACCATALYST
       private int _charPointToUse = 12;
#else
    private int _charPointToUse = 5;
#endif

    private DateTime _selectedDate = DateTime.Now.Date;
    private DateTime _selectedMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    private DateTime _selectedYear = new DateTime(DateTime.Now.Year, 1, 1);

    public LandingPage(IHomeService homeService, ISettingsService settings, IApkUpdateService updateService,
        IErrorHandler errors)
    {
        InitializeComponent();
        _homeService = homeService;
        _settings = settings;
        _updateService = updateService;
        _errors = errors;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_settings.IsOnboardingDone())
        {
            await Shell.Current.GoToAsync("setup");
            return;
        }
        if (!_doneHealthCheck) DoHealthCheckAsync();
        await CheckForUpdateAsync();
        UpdateTabStyles();
        UpdatePickerVisibility();
        await LoadData();
    }

    private async void OnDailyClicked(object sender, EventArgs e)
    {
        if (_currentPeriod == "DAILY") return;
        _currentPeriod = "DAILY";
        _selectedDate = DateTime.Now.Date;
        UpdateTabStyles();
        UpdatePickerVisibility();
        await LoadData();
    }

    private async void OnMonthlyClicked(object sender, EventArgs e)
    {
        if (_currentPeriod == "MONTHLY") return;
        _currentPeriod = "MONTHLY";
        _selectedMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        UpdateTabStyles();
        UpdatePickerVisibility();
        await LoadData();
    }

    private async void OnYearlyClicked(object sender, EventArgs e)
    {
        if (_currentPeriod == "YEARLY") return;
        _currentPeriod = "YEARLY";
        _selectedYear = new DateTime(DateTime.Now.Year, 1, 1);
        UpdateTabStyles();
        UpdatePickerVisibility();
        await LoadData();
    }

    // --- Date picker (daily) ---
    private void OnDatePickerClicked(object sender, EventArgs e)
    {
        HiddenDatePicker.MaximumDate = DateTime.Now.Date;
        HiddenDatePicker.Date = _selectedDate;
        HiddenDatePicker.IsVisible = true;
        HiddenDatePicker.Focus();
    }

    private async void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        HiddenDatePicker.IsVisible = false;
        _selectedDate = e.NewDate.Date;
        UpdatePickerLabels();
        await LoadData();
    }
    // --- Day Nav ---
    private async void OnDayPrev(object sender, EventArgs e)
    {
        _selectedDate = _selectedDate.AddDays(-1);
        UpdatePickerLabels();
        await LoadData();
    }

    private async void OnDayNext(object sender, EventArgs e)
    {
        if (_selectedDate.AddDays(1) > DateTime.Now) return;
        _selectedDate = _selectedDate.AddDays(1);
        UpdatePickerLabels();
        await LoadData();
    }

    // --- Month nav ---
    private async void OnMonthPrev(object sender, EventArgs e)
    {
        _selectedMonth = _selectedMonth.AddMonths(-1);
        UpdatePickerLabels();
        await LoadData();
    }

    private async void OnMonthNext(object sender, EventArgs e)
    {
        if (_selectedMonth.AddMonths(1) > DateTime.Now) return;
        _selectedMonth = _selectedMonth.AddMonths(1);
        UpdatePickerLabels();
        await LoadData();
    }

    // --- Year nav ---
    private async void OnYearPrev(object sender, EventArgs e)
    {
        _selectedYear = _selectedYear.AddYears(-1);
        UpdatePickerLabels();
        await LoadData();
    }

    private async void OnYearNext(object sender, EventArgs e)
    {
        if (_selectedYear.AddYears(1).Year > DateTime.Now.Year) return;
        _selectedYear = _selectedYear.AddYears(1);
        UpdatePickerLabels();
        await LoadData();
    }

    private void UpdatePickerVisibility()
    {
        DailyPickerFrame.IsVisible = _currentPeriod == "DAILY";
        MonthPickerFrame.IsVisible = _currentPeriod == "MONTHLY";
        YearPickerFrame.IsVisible = _currentPeriod == "YEARLY";
        UpdatePickerLabels();
    }

    private void UpdatePickerLabels()
    {
        SelectedDateLabel.Text = _selectedDate.Date == DateTime.Now.Date
            ? "Dziś"
            : _selectedDate.ToString("dd MMMM yyyy");

        SelectedMonthLabel.Text = _selectedMonth.ToString("MMMM yyyy");
        SelectedYearLabel.Text = _selectedYear.ToString("yyyy");
    }

    private async Task DoHealthCheckAsync()
    {
        this._doneHealthCheck = true;
        var isApiHealthy = await _homeService.DoHealthCheck();
        if (!isApiHealthy)
        {
            apiStatus.IsVisible = true;
            apiStatus.Text = "Obecnie występują problemy z dostępnością API. Trwają prace nad przywróceniem pełnej funkcjonalności. Przepraszamy za utrudnienia.";
        }
    }

    private async Task LoadData()
    {
        _cts?.Cancel();
        await Task.Delay(500);
        var cts = _cts = new CancellationTokenSource();

        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        // A reload the user superseded (switching period quickly) cancels the token
        // and is ignored by the handler.
        await _errors.SafeRunAsync(async () =>
        {
            var selectedDate = _currentPeriod switch
            {
                "DAILY" => _selectedDate,
                "MONTHLY" => _selectedMonth,
                "YEARLY" => _selectedYear,
                _ => DateTime.Now
            };

            var vm = await _homeService.GetHomePageViewModel(cts.Token, _currentPeriod, selectedDate);
            BindingContext = vm;

            // Delay so layout settles before setting chart
            Dispatcher.Dispatch(async () =>
            {
                await Task.Delay(100);
                UpdateChart(vm);
            });
        }, "LandingPage.LoadData", ErrorPolicy.Notify);

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
    }

    private void OnChartBarClicked(object sender, EventArgs e)
    {
        _useBarChart = true;
        UpdateChartButtonStyles();
        if (BindingContext is HomePageViewModel vm) UpdateChart(vm);
    }

    private void OnChartLineClicked(object sender, EventArgs e)
    {
        _useBarChart = false;
        UpdateChartButtonStyles();
        if (BindingContext is HomePageViewModel vm) UpdateChart(vm);
    }

    private void UpdateChart(HomePageViewModel vm)
    {
        var points = vm.ChartPoints.TakeLast(_charPointToUse).ToList();
        if (points.Count < 2)
        {
            ChartFrame.IsVisible = false;
            return;
        }
        ChartFrame.IsVisible = true;

#if WINDOWS || MACCATALYST
        RenderScottPlot(points);
#else
        RenderMicrocharts(points);
#endif
    }

    private void RenderMicrocharts(List<ChartPoint> points)
    {
        var entries = points.Select(p => new ChartEntry((float)p.Value)
        {
            Label = p.Label,
            ValueLabel = p.Value.ToString("N0"),
            Color = SKColor.Parse(p.ColorHex ?? "#2E7D32")
        }).ToList();

        bool isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var labelColor = isDark ? SKColor.Parse("#CCCCCC") : SKColor.Parse("#333333");
        var bgColor = SKColors.Transparent;

        Chart chart;
        if (_useBarChart)
        {
            chart = new BarChart
            {
                Entries = entries,
                LabelTextSize = 28,
                LabelColor = labelColor,
                ValueLabelOption = ValueLabelOption.None,
                BarAreaAlpha = 0,
                BackgroundColor = bgColor,
                Margin = 8,
                MinValue = 0
            };
        }
        else
        {
            chart = new LineChart
            {
                Entries = entries,
                LabelTextSize = 28,
                LabelColor = labelColor,
                ValueLabelOption = ValueLabelOption.None,
                LineMode = LineMode.Straight,
                PointMode = PointMode.Circle,
                PointSize = 8,
                LineSize = 3,
                BackgroundColor = bgColor,
                Margin = 8,
                MinValue = 0
            };
        }

        ChartContainer.Children.Clear();
        ChartContainer.Children.Add(new Microcharts.Maui.ChartView
        {
            Chart = chart,
            HeightRequest = 180
        });
    }

#if WINDOWS || MACCATALYST
    private void RenderScottPlot(List<ChartPoint> points)
    {
        bool isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var green = ScottPlot.Color.FromHex("#2E7D32");
        var axisColor = isDark ? ScottPlot.Color.FromHex("#CCCCCC") : ScottPlot.Color.FromHex("#333333");

        var plotView = new ScottPlot.Maui.MauiPlot { HeightRequest = 400, VerticalOptions = LayoutOptions.Fill };
        var plot = plotView.Plot;
        plot.Clear();

        if (_useBarChart)
        {
            var bars = points.Select((p, i) => new ScottPlot.Bar
            {
                Position = i,
                Value = p.Value,
                FillColor = ScottPlot.Color.FromHex(p.ColorHex ?? "#2E7D32")
            }).ToList();
            plot.Add.Bars(bars);
        }
        else
        {
            double[] xs = Enumerable.Range(0, points.Count).Select(i => (double)i).ToArray();
            double[] ys = points.Select(p => p.Value).ToArray();
            var scatter = plot.Add.Scatter(xs, ys);
            scatter.Color = green;
            scatter.LineWidth = 3;
            scatter.MarkerSize = 8;
        }

        ScottPlot.Tick[] ticks = points.Select((p, i) => new ScottPlot.Tick(i, p.Label)).ToArray();
        plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);

        plot.FigureBackground.Color = ScottPlot.Colors.Transparent;
        plot.DataBackground.Color = ScottPlot.Colors.Transparent;
        plot.Axes.Color(axisColor);
        plot.HideGrid();

        ChartContainer.Children.Clear();
        ChartContainer.Children.Add(plotView);
        plotView.Refresh();
    }
#endif

    private void UpdateChartButtonStyles()
    {
        var active = Color.FromArgb("#2E7D32");
        bool isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        ChartBarBtn.BackgroundColor = _useBarChart ? active : Colors.Transparent;
        ChartBarBtn.TextColor = _useBarChart ? Colors.White : (isDark ? Color.FromArgb("#CCCCCC") : Color.FromArgb("#333333"));
        ChartLineBtn.BackgroundColor = !_useBarChart ? active : Colors.Transparent;
        ChartLineBtn.TextColor = !_useBarChart ? Colors.White : (isDark ? Color.FromArgb("#CCCCCC") : Color.FromArgb("#333333"));
    }

    private void UpdateTabStyles()
    {
        var activeBackground = Color.FromArgb("#2E7D32");

        bool isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        var inactiveBackground = Colors.Transparent;
        var activeText = Colors.White;
        var inactiveText = isDark ? Color.FromArgb("#9E9E9E") : Color.FromArgb("#757575");

        TabDaily.BackgroundColor = _currentPeriod == "DAILY" ? activeBackground : inactiveBackground;
        TabMonthly.BackgroundColor = _currentPeriod == "MONTHLY" ? activeBackground : inactiveBackground;
        TabYearly.BackgroundColor = _currentPeriod == "YEARLY" ? activeBackground : inactiveBackground;

        TabDailyLabel.TextColor = _currentPeriod == "DAILY" ? activeText : inactiveText;
        TabMonthlyLabel.TextColor = _currentPeriod == "MONTHLY" ? activeText : inactiveText;
        TabYearlyLabel.TextColor = _currentPeriod == "YEARLY" ? activeText : inactiveText;
    }
    private async Task CheckForUpdateAsync()
    {
        var latest = await _updateService.GetLatestVersionInfoAsync();

        if (latest is null || !latest.IsCached)
        {
            return; // brak polaczenia albo serwer jeszcze nic nie zcache'owal
        }

        string currentVersion = _updateService.GetCurrentAppVersion();

        if (!_updateService.IsNewerVersionAvailable(latest.VersionName, currentVersion))
        {
            return; 
        }

        bool shouldUpdate = await DisplayAlert(
            "Dostępna aktualizacja",
            $"Nowa wersja {latest.VersionName} jest dostępna (masz {currentVersion}). Pobrać teraz?",
            "Tak",
            "Nie teraz");

        if (shouldUpdate)
        {
            await DownloadAndInstallAsync();
        }
    }
    private async Task DownloadAndInstallAsync()
    {
        DownloadProgressFrame.IsVisible = true;
        DownloadProgressBar.Progress = 0;
        DownloadProgressLabel.Text = "Pobieranie... 0%";

        await _errors.SafeRunAsync(async () =>
        {
            var progress = new Progress<double>(percent =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DownloadProgressBar.Progress = percent / 100.0;
                    DownloadProgressLabel.Text = $"Pobieranie... {percent:F0}%";
                });
            });

            string filePath = await _updateService.DownloadApkAsync(progress);

            DownloadProgressLabel.Text = "Pobrano, instalowanie...";

#if ANDROID
            ApkInstaller.InstallApk(filePath);
#endif
        }, "LandingPage.DownloadAndInstallAsync", ErrorPolicy.Notify);

        DownloadProgressFrame.IsVisible = false;
    }
}
