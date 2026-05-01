using System;
using System.Threading.Tasks;

namespace ScCestinator.Services;

public record AppUpdateResult(
    bool IsUpdateAvailable,
    string LatestVersion,
    string StatusMessage,
    string? DownloadUrl = null);

public class AppUpdateService
{
    private readonly GitHubService _gitHubService = new();

    public async Task<AppUpdateResult> CheckForUpdateAsync(string currentVersion)
    {
        try
        {
            var latestAppVersion = await _gitHubService.GetLatestAppVersionAsync();

            if (!string.IsNullOrWhiteSpace(latestAppVersion) && latestAppVersion != currentVersion)
            {
                return new AppUpdateResult(
                    IsUpdateAvailable: true,
                    LatestVersion: latestAppVersion,
                    StatusMessage: $"⚠ Nová verze aplikace: {currentVersion} → {latestAppVersion}");
            }

            return new AppUpdateResult(
                IsUpdateAvailable: false,
                LatestVersion: latestAppVersion ?? string.Empty,
                StatusMessage: string.Empty);
        }
        catch
        {
            // Nechceme rozbíjet start aplikace kvůli kontrole aktualizace
            return new AppUpdateResult(
                IsUpdateAvailable: false,
                LatestVersion: string.Empty,
                StatusMessage: string.Empty);
        }
    }
}
