using KAMICH.Core.Models;

namespace KAMICH.Core.Services;

public interface IHomeService
{
    Task<HomePageViewModel> GetHomePageViewModel(CancellationToken cts);
    Task<HomePageViewModel> GetHomePageViewModel(CancellationToken cts, string period);
    Task<HomePageViewModel> GetHomePageViewModel(CancellationToken cts, string period, DateTime selectedDate);
    Task<bool> DoHealthCheck();
}