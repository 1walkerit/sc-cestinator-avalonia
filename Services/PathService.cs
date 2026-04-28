using System;
using System.IO;

namespace ScCestinator.Services;

public sealed class PathValidationResult
{
    public string? LivePath { get; init; }
    public string GlobalIniPath { get; init; } = string.Empty;

    public bool LiveExists { get; init; }
    public bool DataExists { get; init; }
    public bool LocalizationExists { get; init; }
    public bool GlobalIniExists { get; init; }

    public bool IsValidLivePath => LiveExists && DataExists;
    public bool HasInstalledLocalization => GlobalIniExists;
}

public sealed class PathService
{
    public PathValidationResult ValidateStarCitizenPath(string? inputPath)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            return new PathValidationResult();

        var normalizedPath = Path.GetFullPath(inputPath.Trim());

        string livePath;

        if (string.Equals(Path.GetFileName(normalizedPath), "LIVE", StringComparison.OrdinalIgnoreCase))
        {
            livePath = normalizedPath;
        }
        else
        {
            livePath = Path.Combine(normalizedPath, "LIVE");
        }

        var dataPath = Path.Combine(livePath, "data");
        var localizationPath = Path.Combine(dataPath, "Localization");
        var englishPath = Path.Combine(localizationPath, "english");
        var globalIniPath = Path.Combine(englishPath, "global.ini");

        return new PathValidationResult
        {
            LivePath = livePath,
            GlobalIniPath = globalIniPath,
            LiveExists = Directory.Exists(livePath),
            DataExists = Directory.Exists(dataPath),
            LocalizationExists = Directory.Exists(localizationPath),
            GlobalIniExists = File.Exists(globalIniPath)
        };
    }
}
