using KAMICH.Core.Models;

namespace KAMICH.Core.Services.Implementations;

public class HomeService : IHomeService
{
    private readonly IMemoryService _memoryService;
    private readonly IAnalysisService _analysisService;

    public HomeService(IMemoryService memoryService, IAnalysisService analysisService)
    {
        _memoryService = memoryService;
        _analysisService = analysisService;
    }

    public async Task<HomePageViewModel> GetHomePageViewModel()
    {
        return await GetViewModel("DAILY");
    }

    public async Task<HomePageViewModel> GetHomePageViewModel(string period)
    {
        return await GetViewModel(period);
    }

    private async Task<HomePageViewModel> GetViewModel(string period)
    {
        var trackedVehicles = _memoryService.GetMemoryVehicles().Where(x => x.IsTracked).ToList();
        if (trackedVehicles.Count == 0)
            return new HomePageViewModel { PeriodLabel = GetLabel(period) };

        double totalIncome;

        if (period == "DAILY")
        {
            totalIncome = await _analysisService.CalculateDailyIncome(trackedVehicles);
        }
        else
        {
            totalIncome = await _analysisService.CalculateIncomeFromStats(trackedVehicles, period);
        }

        var vms = trackedVehicles.Select(item => new SimpleVehicleVm
        {
            Name = item.Name,
            Id = item.Id,
            Income = _analysisService.GetIncomeForVehicle(item.Id),
            Color = item.CustomColor,
            Icon = item.CustomIcon
        }).ToList();

        return new HomePageViewModel
        {
            PeriodLabel = GetLabel(period),
            TotalIncome = totalIncome,
            Vehicles = vms
        };
    }

    private static string GetLabel(string period) => period switch
    {
        "DAILY" => "Przychód dzienny",
        "MONTHLY" => "Przychód miesięczny",
        "YEARLY" => "Przychód roczny",
        _ => "Przychód"
    };
}
