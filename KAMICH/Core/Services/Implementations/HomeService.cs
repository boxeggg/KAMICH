using KAMICH.Core.Models;
using KAMICH.Core.ViewModels;

namespace KAMICH.Core.Services.Implementations;

public class HomeService : IHomeService
{
    private readonly IMemoryService _memoryService;
    private readonly IAnalysisService _analysisService;
    private readonly ISettingsService _settings;
    private readonly IHealthCheckService _healthCheckService;
    private readonly IIncomeHistoryService _historyService;

    public HomeService(IMemoryService memoryService, IAnalysisService analysisService,
        ISettingsService settings, IHealthCheckService healthCheckService,
        IIncomeHistoryService historyService)
    {
        _memoryService = memoryService;
        _analysisService = analysisService;
        _settings = settings;
        _healthCheckService = healthCheckService;
        _historyService = historyService;
    }

    public async Task<bool> DoHealthCheck()
    {
        try
        {
            return await _healthCheckService.IsApiHealthy();
        }
        catch
        {
            return false;
        }
    }

    public async Task<HomePageViewModel> GetHomePageViewModel(CancellationToken cts)
    {
        return await GetViewModel(cts, "DAILY", DateTime.Now.Date);
    }

    public async Task<HomePageViewModel> GetHomePageViewModel(CancellationToken cts, string period)
    {
        return await GetViewModel(cts, period, DateTime.Now.Date);
    }

    public async Task<HomePageViewModel> GetHomePageViewModel(CancellationToken cts, string period, DateTime selectedDate)
    {
        return await GetViewModel(cts, period, selectedDate);
    }

    private async Task<HomePageViewModel> GetViewModel(CancellationToken cts, string period, DateTime selectedDate)
    {
        var trackedVehicles = _memoryService.GetMemoryVehicles().Where(x => x.IsTracked).ToList();
        if (trackedVehicles.Count == 0)
            return new HomePageViewModel { PeriodLabel = GetLabel(period, selectedDate) };

        double totalIncome;

        if (period == "DAILY" && selectedDate.Date == DateTime.Now.Date)
        {
            totalIncome = await _analysisService.CalculateDailyIncome(cts, trackedVehicles);
        }
        else if (period == "DAILY")
        {
            totalIncome = await _analysisService.CalculateIncomeFromWorkLogs(cts, trackedVehicles, selectedDate);
        }
        else
        {
            totalIncome = await _analysisService.CalculateIncomeFromStatsByPeriod(cts, trackedVehicles, period, selectedDate);
        }

        var vms = trackedVehicles.Select(item => new SimpleVehicleVm
        {
            Name = item.Name,
            Id = item.Id,
            Income = _analysisService.GetIncomeForVehicle(cts, item.Id),
            Color = item.CustomColor,
            Icon = item.CustomIcon
        }).ToList();

        // Save to history
        await _historyService.SaveEntry(new IncomeHistoryEntry
        {
            Date = NormalizeDate(period, selectedDate),
            Period = period,
            Income = totalIncome
        });

        var appSettings = await _settings.LoadAsync();
        var monthly = appSettings.MonthlyFixedCosts;
        var multiplier = GetCostMultiplier(period);

        // Build chart
        var history = _historyService.GetHistory(period,31);
        var chartPoints = history.Select(h => new ChartPoint(
            GetChartLabel(period, h.Date),
            h.Income,
            "#2E7D32")).ToList();

        var periodLabel = GetLabel(period, selectedDate);

        // Update Android widget only with today's daily income
#if ANDROID
        if (period == "DAILY" && selectedDate.Date == DateTime.Now.Date)
        {
            WidgetHelper.UpdateWidgetData(totalIncome, "Przychód dzienny");
        }
#endif

        return new HomePageViewModel
        {
            PeriodLabel = periodLabel,
            TotalIncome = totalIncome,
            Vehicles = vms,
            FixedCostHotel = monthly.Hotel * multiplier,
            FixedCostTransport = monthly.Transport * multiplier,
            FixedCostService = monthly.Service * multiplier,
            FixedCostOther = monthly.Other * multiplier,
            ChartPoints = chartPoints
        };
    }

    private static DateTime NormalizeDate(string period, DateTime date) => period switch
    {
        "DAILY" => date.Date,
        "MONTHLY" => new DateTime(date.Year, date.Month, 1),
        "YEARLY" => new DateTime(date.Year, 1, 1),
        _ => date.Date
    };

    private static string GetChartLabel(string period, DateTime date) => period switch
    {
        "DAILY" => date.ToString("dd"),
        "MONTHLY" => date.ToString("MMM"),
        "YEARLY" => date.ToString("yy"),
        _ => date.ToString("dd")
    };

    private static double GetCostMultiplier(string period) => period switch
    {
        "DAILY" => 1.0 / 30.0,
        "MONTHLY" => 1.0,
        "YEARLY" => 12.0,
        _ => 1.0
    };

    private static string GetLabel(string period, DateTime date) => period switch
    {
        "DAILY" when date.Date == DateTime.Now.Date => "Przychód dzienny",
        "DAILY" => $"Przychód za {date:dd.MM.yyyy}",
        "MONTHLY" => $"Przychód za {date:MMMM yyyy}",
        "YEARLY" => $"Przychód za {date:yyyy}",
        _ => "Przychód"
    };
}
