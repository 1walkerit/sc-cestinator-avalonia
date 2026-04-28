using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScCestinator.Services;

public sealed class LocalizationService
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    public async Task<string?> ReadLocalVersionAsync(string globalIniPath)
    {
        if (string.IsNullOrWhiteSpace(globalIniPath))
            return null;

        if (!File.Exists(globalIniPath))
            return null;

        await using var stream = File.OpenRead(globalIniPath);
        using var reader = new StreamReader(stream);

        var firstLine = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(firstLine))
            return null;

        return firstLine.Trim().TrimStart(';').Trim();
    }

    public async Task<bool> InstallOrUpdateAsync(string livePath, bool createBackup)
    {
        try
        {
            var dataPath = Path.Combine(livePath, "data");
            var localizationPath = Path.Combine(dataPath, "Localization");
            var englishPath = Path.Combine(localizationPath, "english");
            var globalIniPath = Path.Combine(englishPath, "global.ini");

            // 🔹 vytvoření složek
            Directory.CreateDirectory(englishPath);

            // 🔹 záloha
            if (createBackup && Directory.Exists(localizationPath))
            {
                var backupPath = localizationPath + ".bak";

                if (Directory.Exists(backupPath))
                    Directory.Delete(backupPath, true);

                DirectoryCopy(localizationPath, backupPath);
            }

            // 🔹 stažení souboru
            var content = await _httpClient.GetStringAsync(Constants.GitHubLocalizationUrl);

            // 🔹 uložení
            await File.WriteAllTextAsync(globalIniPath, content);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private void DirectoryCopy(string sourceDir, string destDir)
    {
        var dir = new DirectoryInfo(sourceDir);

        Directory.CreateDirectory(destDir);

        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        foreach (var subDir in dir.GetDirectories())
        {
            var newDestDir = Path.Combine(destDir, subDir.Name);
            DirectoryCopy(subDir.FullName, newDestDir);
        }
    }
}