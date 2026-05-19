using KAMICH.Core.Models;

namespace KAMICH.Core.Services;

public interface IHomeService
{
    Task<HomePageViewModel> GetHomePageViewModel();
    Task<HomePageViewModel> GetHomePageViewModel(string period);
}