using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScCestinator.Services;

public sealed class PathValidationResult
{
    public string? LivePath { get; init; }
    public string? BranchName { get; init; }
    public string GlobalIniPath { get; init; } = string.Empty;

    public bool LiveExists { get; init; }
    public bool DataP4kExists { get; init; }
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
        return ValidateStarCitizenPath(inputPath, "LIVE");
    }

    public PathValidationResult ValidateStarCitizenPath(string? inputPath, string branchName)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            return new PathValidationResult();

        var normalizedPath = Path.GetFullPath(inputPath.Trim());
        var normalizedBranch = NormalizeBranchName(branchName);
        var installationRootPath = GetInstallationRootPath(normalizedPath);

        var branchPath = Path.Combine(installationRootPath, normalizedBranch);

        var dataP4kPath = Path.Combine(branchPath, "Data.p4k");
        var dataPath = Path.Combine(branchPath, "data");
        var localizationPath = Path.Combine(dataPath, "Localization");
        var englishPath = Path.Combine(localizationPath, "english");
        var globalIniPath = Path.Combine(englishPath, "global.ini");

        return new PathValidationResult
        {
            LivePath = branchPath,
            BranchName = normalizedBranch,
            GlobalIniPath = globalIniPath,
            LiveExists = Directory.Exists(branchPath),
            DataExists = Directory.Exists(dataPath),
            LocalizationExists = Directory.Exists(localizationPath),
            DataP4kExists = File.Exists(dataP4kPath),
            GlobalIniExists = File.Exists(globalIniPath)
        };
    }

    public IReadOnlyList<string> DetectExistingBranches(string? inputPath)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            return Array.Empty<string>();

        var normalizedPath = Path.GetFullPath(inputPath.Trim());
        var installationRootPath = GetInstallationRootPath(normalizedPath);
        var candidates = new List<string>(Constants.SupportedStarCitizenBranches.Length);

        foreach (var branch in Constants.SupportedStarCitizenBranches)
        {
            var branchPath = Path.Combine(installationRootPath, branch);

            if (Directory.Exists(branchPath))
            {
                candidates.Add(branch);
            }
        }

        return candidates;
    }

    private static string GetInstallationRootPath(string normalizedPath)
    {
        var directoryName = Path.GetFileName(
            normalizedPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        var isBranchPath = Constants.SupportedStarCitizenBranches
            .Any(x => string.Equals(x, directoryName, StringComparison.OrdinalIgnoreCase));

        if (!isBranchPath)
            return normalizedPath;

        return Directory.GetParent(normalizedPath)?.FullName ?? normalizedPath;
    }

    private static string NormalizeBranchName(string? branchName)
    {
        if (string.IsNullOrWhiteSpace(branchName))
            return "LIVE";

        var match = Constants.SupportedStarCitizenBranches
            .FirstOrDefault(x => string.Equals(x, branchName.Trim(), StringComparison.OrdinalIgnoreCase));

        return match ?? "LIVE";
    }
}
