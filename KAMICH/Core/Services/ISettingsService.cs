using KAMICH.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAMICH.Core.Services.Implementations
{
    public interface ISettingsService
    {
        Task<AppSettingsModel> LoadAsync();
        Task SaveAsync(AppSettingsModel model);
        Task<string?> GetApiKeyAsync();
        Task SetApiKeyAsync(string? apiKey);
        void ResetNonSecrets();
        void ResetAll();
    }
}
