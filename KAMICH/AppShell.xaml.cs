using KAMICH.Pages;

namespace KAMICH
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("details", typeof(VehicleDetails));
            Routing.RegisterRoute("customization", typeof(VehicleCustomizationPage));
        }
    }
}
