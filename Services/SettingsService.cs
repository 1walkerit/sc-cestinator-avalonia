using System;
using System.IO;
using System.Text.Json;

namespace ScCestinator.Services;

public class AppSettings
{
    public string? LastUsedPath { get; set; }
}

public class SettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ScCestinator",
        "settings.json"
    );

    public AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch
        {
            // If loading fails, return default settings
        }

        return new AppSettings();
    }

    public void SaveSettings(AppSettings settings)
    {
        try
        {
            var directory = Path.GetDirectoryName(SettingsPath);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(SettingsPath, json);
        }
        catch
        {
            // Silently fail if saving fails
        }
    }
}
