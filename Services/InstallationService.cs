using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScCestinator.Services;

public sealed class InstallationService
{
    private const int MaxDepth = 3;
    private static readonly string[] RelativeTargets =
    {
        Path.Combine("drive_c", "Program Files", "Roberts Space Industries", "StarCitizen"),
        Path.Combine("drive_c", "Program Files (x86)", "Roberts Space Industries", "StarCitizen")
    };

    private static readonly string[] DirectInstallCandidates =
    {
        "StarCitizen",
        "star-citizen",
        Path.Combine("Games", "StarCitizen"),
        Path.Combine("Games", "star-citizen"),
        Path.Combine(".local", "share", "StarCitizen"),
        Path.Combine(".local", "share", "star-citizen"),
        Path.Combine(".local", "share", "lug-helper", "StarCitizen"),
        Path.Combine(".local", "share", "lug-helper", "star-citizen")
    };

    public Task<List<string>> FindStarCitizenInstallationsAsync()
    {
        return Task.Run(FindStarCitizenInstallations);
    }

    public List<string> FindStarCitizenInstallations()
    {
        var found = new HashSet<string>(StringComparer.Ordinal);
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (!string.IsNullOrWhiteSpace(home))
        {
            foreach (var relative in DirectInstallCandidates)
            {
                var directCandidate = Path.Combine(home, relative);
                if (IsValidInstallationRoot(directCandidate))
                {
                    found.Add(Path.GetFullPath(directCandidate));
                }
            }
        }

        var roots = GetSearchRoots().Distinct(StringComparer.Ordinal);

        foreach (var root in roots)
        {
            if (!Directory.Exists(root))
                continue;

            ScanDirectory(root, 0, found);
        }

        return new List<string>(found);
    }

    private static IEnumerable<string> GetSearchRoots()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (!string.IsNullOrWhiteSpace(home))
        {
            // Common Wine prefixes
            yield return Path.Combine(home, ".wine");
            yield return Path.Combine(home, ".local", "share", "lutris", "prefixes");
            yield return Path.Combine(home, ".local", "share", "Steam", "steamapps", "compatdata");

            // Common game folders (including LUG Helper typical location)
            yield return Path.Combine(home, "Games");
            yield return Path.Combine(home, ".local", "share");
        }
    }

    private static void ScanDirectory(string currentDir, int depth, HashSet<string> found)
    {
        try
        {
            foreach (var relativeTarget in RelativeTargets)
            {
                var candidate = Path.Combine(currentDir, relativeTarget);
                if (IsValidInstallationRoot(candidate))
                {
                    found.Add(Path.GetFullPath(candidate));
                }
            }

            if (IsValidInstallationRoot(currentDir))
            {
                found.Add(Path.GetFullPath(currentDir));
            }

            if (depth >= MaxDepth)
                return;

            IEnumerable<string> subdirs;
            try
            {
                subdirs = Directory.EnumerateDirectories(currentDir);
            }
            catch
            {
                return;
            }

            foreach (var subdir in subdirs)
            {
                ScanDirectory(subdir, depth + 1, found);
            }
        }
        catch
        {
            // Ignore inaccessible/unexpected paths and continue safely.
        }
    }

    private static bool IsValidInstallationRoot(string candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate) || !Directory.Exists(candidate))
            return false;

        var livePath = Path.Combine(candidate, "LIVE");
        var dataP4kPath = Path.Combine(livePath, "Data.p4k");

        return Directory.Exists(livePath)
               && File.Exists(dataP4kPath);
    }
}
