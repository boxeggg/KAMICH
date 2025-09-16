using KAMICH.Pages;

namespace KAMICH
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("fleet", typeof(FleetPage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("landing", typeof(LandingPage));
        }
    }
}
