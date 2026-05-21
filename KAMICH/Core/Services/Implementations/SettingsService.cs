using KAMICH.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAMICH.Core.Services.Implementations
{
    public class SettingsService : ISettingsService
    {
        const string KeyDarkMode = "settings.dark_mode";
        const string FuelPrice = "settings.fuel_price";
        const string HourlyPrice = "settings.hourly_price";
        const string OperatorPrice = "settings.operator_price";
        const string FuelConsumption = "settings.fuel_consumption";
        const string KeyApiKey = "settings.external_api_key";
        const string KeyOnboardingDone = "settings.onboarding_done";
        const string KeyFixedHotel = "settings.fixed_hotel";
        const string KeyFixedTransport = "settings.fixed_transport";
        const string KeyFixedService = "settings.fixed_service";
        const string KeyFixedOther = "settings.fixed_other";

        public async Task<AppSettingsModel> LoadAsync()
        {
            var model = new AppSettingsModel
            {
                DarkMode = Preferences.Get(KeyDarkMode, false),
                GlobalVehicleSettings = new VehicleStatsModel {
                    FuelConsumptionPerHour = Preferences.Get(FuelConsumption, 0.0),
                    FuelPrice = Preferences.Get(FuelPrice, 0.0),
                    OperatorPrice = Preferences.Get(OperatorPrice, 0.0),
                    HourlyPrice = Preferences.Get(HourlyPrice, 0.0),
                },
                ApiKey = await TryGetSecureAsync(KeyApiKey),
                MonthlyFixedCosts = new FixedCostsModel
                {
                    Hotel = Preferences.Get(KeyFixedHotel, 0.0),
                    Transport = Preferences.Get(KeyFixedTransport, 0.0),
                    Service = Preferences.Get(KeyFixedService, 0.0),
                    Other = Preferences.Get(KeyFixedOther, 0.0),
                }
            };
            return model;
        }

        public async Task SaveAsync(AppSettingsModel model)
        {
            Preferences.Set(KeyDarkMode, model.DarkMode);
            Preferences.Set(FuelPrice, model.GlobalVehicleSettings.FuelPrice);
            Preferences.Set(HourlyPrice, model.GlobalVehicleSettings.HourlyPrice);
            Preferences.Set(OperatorPrice, model.GlobalVehicleSettings.OperatorPrice);
            Preferences.Set(FuelConsumption, model.GlobalVehicleSettings.FuelConsumptionPerHour);
            Preferences.Set(KeyFixedHotel, model.MonthlyFixedCosts.Hotel);
            Preferences.Set(KeyFixedTransport, model.MonthlyFixedCosts.Transport);
            Preferences.Set(KeyFixedService, model.MonthlyFixedCosts.Service);
            Preferences.Set(KeyFixedOther, model.MonthlyFixedCosts.Other);
            await SetApiKeyAsync(model.ApiKey);
        }

        public async Task<string?> GetApiKeyAsync() => await TryGetSecureAsync(KeyApiKey);

        public async Task SetApiKeyAsync(string? apiKey)
        {
            var value = apiKey?.Trim();
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    SecureStorage.Remove(KeyApiKey);
                    Preferences.Set(KeyOnboardingDone, false);
                }
                else
                {
                    await SecureStorage.SetAsync(KeyApiKey, value);
                    Preferences.Set(KeyOnboardingDone, true);
                }

            }
            catch
            {
            }
        }

        public bool IsOnboardingDone() => Preferences.Get(KeyOnboardingDone, false);

        public void ResetNonSecrets()
        {
            Preferences.Remove(KeyDarkMode);
        }

        public void ResetAll()
        {
            Preferences.Clear();
            try { SecureStorage.Remove(KeyApiKey); } catch { }
        }

        static async Task<string?> TryGetSecureAsync(string key)
        {
            try { return await SecureStorage.GetAsync(key); }
            catch { return null; }
        }
    }
}
