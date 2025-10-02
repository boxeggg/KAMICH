using KAMICH.Core.Services;
using KAMICH.Pages;

namespace KAMICH.Pages
{
    public partial class LandingPage : ContentPage
    {
        private IHomeService _homeService;
        public LandingPage(IHomeService homeService)
        {
            InitializeComponent();
            _homeService = homeService;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var vm = await _homeService.GetHomePageViewModel();
                BindingContext = vm;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
        }


        private async void OnFleetClicked(object sender, EventArgs e)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync("fleet");
                    return;
                }
            }
            catch
            {
            }

        }


        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync("settings");
                    return;
                }
            }
            catch
            {
            }

        }
    }
}
