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
        catch (JsonException)
        {
            // Corrupted settings file - return defaults
            return new AppSettings();
        }
        catch (IOException)
        {
            // File access error - return defaults
            return new AppSettings();
        }
        catch (UnauthorizedAccessException)
        {
            // Permission error - return defaults
            return new AppSettings();
        }

        return new AppSettings();
    }

    public bool SaveSettings(AppSettings settings)
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
            return true;
        }
        catch (IOException)
        {
            // File I/O error - fail gracefully
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            // Permission error - fail gracefully
            return false;
        }
        catch (JsonException)
        {
            // Serialization error - fail gracefully
            return false;
        }
    }
}
