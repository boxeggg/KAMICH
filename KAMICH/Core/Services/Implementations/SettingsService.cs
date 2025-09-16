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
        const string KeyApiKey = "settings.external_api_key";

        public async Task<AppSettingsModel> LoadAsync()
        {
            var model = new AppSettingsModel
            {
                DarkMode = Preferences.Get(KeyDarkMode, false),
                GlobalFuelPrice = Preferences.Get(FuelPrice, 0.0),
                GlobalHourlyPrice = Preferences.Get(HourlyPrice, 0.0),
                GlobalOperatorPrice = Preferences.Get(OperatorPrice, 0.0),
                ApiKey = await TryGetSecureAsync(KeyApiKey)
            };
            return model;
        }

        public async Task SaveAsync(AppSettingsModel model)
        {
            Preferences.Set(KeyDarkMode, model.DarkMode);
            Preferences.Set(FuelPrice, model.GlobalFuelPrice);
            Preferences.Set(HourlyPrice, model.GlobalHourlyPrice);
            Preferences.Set(OperatorPrice, model.GlobalOperatorPrice);
            await SetApiKeyAsync(model.ApiKey);
        }

        public async Task<string?> GetApiKeyAsync() => await TryGetSecureAsync(KeyApiKey);

        public async Task SetApiKeyAsync(string? apiKey)
        {
            var value = apiKey?.Trim();
            try
            {
                if (string.IsNullOrEmpty(value))
                    SecureStorage.Remove(KeyApiKey);
                else
                    await SecureStorage.SetAsync(KeyApiKey, value);
            }
            catch
            {
            }
        }

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
