using CommunityToolkit.Maui;
using KAMICH.Core.Services;
using KAMICH.Core.Services.Implementations;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
#if WINDOWS || MACCATALYST
using ScottPlot.Maui;
#endif

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
                .UseMicrocharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                    fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                    fonts.AddFont("fa-solid-900.ttf", "FASolid");
                })
                .ConfigureMauiHandlers(handlers =>
                {
#if IOS
                    Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("DismissKeyboard", (handler, view) =>
                    {
                        handler.PlatformView.ShouldReturn = textField =>
                        {
                            textField.ResignFirstResponder();
                            return true;
                        };
                    });
#endif
                });

#if WINDOWS || MACCATALYST
            builder.UseScottPlot();
#endif
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<IMemoryService, MemoryService>();
            builder.Services.AddSingleton<ICacheService, CacheService>();
            builder.Services.AddTransient<IHomeService, HomeService>();
            builder.Services.AddSingleton<IAnalysisService, AnalysisService>();
            builder.Services.AddSingleton<IIncomeHistoryService, IncomeHistoryService>();
            builder.Services.AddTransient<IVehicleService, VehicleService>();
            builder.Services.AddTransient<IHealthCheckService, HealthCheckService>();
            builder.Services.AddTransient<Pages.StatsPage>();
            builder.Services.AddTransient<Pages.SetupPage>();
            builder.Services.AddTransient<Pages.LandingPage>();
            builder.Services.AddSingleton<HttpClient>(sp => new HttpClient
            {
#if DEBUG
                 // BaseAddress = new Uri("http://10.0.2.2:8080/"),
                 BaseAddress = new Uri("https://kamich-api-production.up.railway.app/"),
#else
                BaseAddress = new Uri("https://kamich-api-production.up.railway.app/"),
#endif
                Timeout = TimeSpan.FromSeconds(60)
            });



            var mauiApp = builder.Build();
            return mauiApp;


#if DEBUG
            builder.Logging.AddDebug();
            
#endif

            
        }
    }
}
