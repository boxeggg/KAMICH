using KAMICH.Integrations.Linqo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KAMICH.Core.Services.Implementations
{
    public interface IApkUpdateService
    {
        Task<VersionInfoDto?> GetLatestVersionInfoAsync();

        string GetCurrentAppVersion();

        bool IsNewerVersionAvailable(string remoteVersion, string currentVersion);

        Task<string> DownloadApkAsync(IProgress<double>? progress = null);
    }
}
