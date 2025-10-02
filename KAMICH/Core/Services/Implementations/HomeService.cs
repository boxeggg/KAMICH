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
        var trackedVehicles =  _memoryService.GetMemoryVehicles().Where(x => x.IsTracked == true).ToList();
        if (trackedVehicles.Count == 0) return new HomePageViewModel();
        var dailyIncome = await _analysisService.CalculateDailyIncome(trackedVehicles);
        
        List<SimpleVehicleVm> vms = new List<SimpleVehicleVm>(); 
        foreach (var item in trackedVehicles)
        {
            var model = new SimpleVehicleVm
            {
                Name = item.Name,
                Id = item.Id,
                Income = _analysisService.GetIncomeForVehicle(item.Id),
                Color = item.CustomColor,
                Icon = item.CustomIcon
            };
            vms.Add(model);
        }

        var vm = new HomePageViewModel()
        {
            DailyIncome = dailyIncome,
            Vehicles = vms
        };
        return vm; 
    }
}