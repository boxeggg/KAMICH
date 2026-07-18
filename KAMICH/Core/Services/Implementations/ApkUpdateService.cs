using KAMICH.Integrations.Linqo.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace KAMICH.Core.Services.Implementations
{
    public class ApkUpdateService : IApkUpdateService
    {
        private readonly HttpClient _httpClient;

        public ApkUpdateService(HttpClient http)
        {
            _httpClient = http;
        }

        public async Task<VersionInfoDto?> GetLatestVersionInfoAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<VersionInfoDto>("api/apk/version");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Blad pobierania wersji: {ex.Message}");
                return null;
            }
        }

        public string GetCurrentAppVersion()
        {
            return AppInfo.Current.VersionString;
        }

        public bool IsNewerVersionAvailable(string remoteVersion, string currentVersion)
        {
            string remote = remoteVersion.TrimStart('v', 'V');
            string current = currentVersion.TrimStart('v', 'V');

            if (Version.TryParse(remote, out var remoteVer) &&
                Version.TryParse(current, out var currentVer))
            {
                return remoteVer > currentVer;
            }

            return !string.Equals(remote, current, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string> DownloadApkAsync(IProgress<double>? progress = null)
        {
            string targetPath = Path.Combine(FileSystem.CacheDirectory, "update.apk");

            using var response = await _httpClient.GetAsync(
                "api/apk/latest",
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(
                targetPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            long totalRead = 0;
            int read;

            while ((read = await contentStream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read));
                totalRead += read;

                if (totalBytes.HasValue && progress != null)
                {
                    progress.Report((double)totalRead / totalBytes.Value * 100);
                }
            }

            return targetPath;
        }

    }
}
