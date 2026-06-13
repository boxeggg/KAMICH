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
            Routing.RegisterRoute("stats", typeof(StatsPage));
            Routing.RegisterRoute("setup", typeof(SetupPage));

            BuildNavigation();
        }

        private void BuildNavigation()
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
            {
                FlyoutBehavior = FlyoutBehavior.Flyout;
                Items.Add(MakeFlyoutItem("Home", "home", 0xF015, typeof(LandingPage)));
                Items.Add(MakeFlyoutItem("Flota", "fleet", 0xF1B9, typeof(FleetPage)));
                Items.Add(MakeFlyoutItem("Ustawienia", "settings", 0xF013, typeof(SettingsPage)));
            }
            else
            {
                FlyoutBehavior = FlyoutBehavior.Disabled;
                var tabBar = new TabBar();
                tabBar.Items.Add(MakeTab("Home", "home", 0xF015, typeof(LandingPage)));
                tabBar.Items.Add(MakeTab("Flota", "fleet", 0xF1B9, typeof(FleetPage)));
                tabBar.Items.Add(MakeTab("Ustawienia", "settings", 0xF013, typeof(SettingsPage)));
                Items.Add(tabBar);
            }
        }

        private static FontImageSource Icon(int codepoint) =>
            new FontImageSource { Glyph = char.ConvertFromUtf32(codepoint), FontFamily = "FASolid", Size = 20 };

        private static ShellContent MakeTab(string title, string route, int glyph, Type page) =>
            new ShellContent
            {
                Title = title,
                Route = route,
                Icon = Icon(glyph),
                ContentTemplate = new DataTemplate(page)
            };

        private static FlyoutItem MakeFlyoutItem(string title, string route, int glyph, Type page)
        {
            var item = new FlyoutItem
            {
                Title = title,
                Route = route,
                Icon = Icon(glyph)
            };
            item.Items.Add(new ShellContent
            {
                Title = title,
                ContentTemplate = new DataTemplate(page)
            });
            return item;
        }
    }
}
