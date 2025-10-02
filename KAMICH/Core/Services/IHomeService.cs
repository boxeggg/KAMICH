using KAMICH.Core.Models;

namespace KAMICH.Core.Services;

public interface IHomeService
{
    public Task<HomePageViewModel>  GetHomePageViewModel();
}