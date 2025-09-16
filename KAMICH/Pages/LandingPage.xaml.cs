using KAMICH.Pages;

namespace KAMICH.Pages
{
    public partial class LandingPage : ContentPage
    {
        public LandingPage()
        {
            InitializeComponent();
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
