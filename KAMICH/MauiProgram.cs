using CommunityToolkit.Maui;
using KAMICH.Core.Services;
using KAMICH.Core.Services.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;

namespace KAMICH
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                    fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                    fonts.AddFont("fa-solid-900.ttf", "FASolid");
                });
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<IMemoryService, MemoryService>();
            builder.Services.AddSingleton<ICacheService, CacheService>();
            builder.Services.AddTransient<IHomeService, HomeService>();
            builder.Services.AddSingleton<IAnalysisService, AnalysisService>();
            builder.Services.AddTransient<IVehicleService, VehicleService>();
            builder.Services.AddSingleton<HttpClient>(sp => new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:8080/"),
                Timeout = TimeSpan.FromSeconds(30)
            });



            var mauiApp = builder.Build();
            return mauiApp;


#if DEBUG
            builder.Logging.AddDebug();
            
#endif

            
        }
    }
}
