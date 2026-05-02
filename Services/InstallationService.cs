using System;
using System.Collections.Generic;
using System.IO;

namespace ScCestinator.Services;

public sealed class InstallationService
{
    private const int MaxDepth = 5;
    private static readonly string[] RelativeTargets =
    {
        Path.Combine("drive_c", "Program Files", "Roberts Space Industries", "StarCitizen")
    };

    public List<string> FindStarCitizenInstallations()
    {
        var found = new HashSet<string>(StringComparer.Ordinal);
        var roots = GetSearchRoots();

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
            yield return Path.Combine(home, "Games");
            yield return Path.Combine(home, ".local", "share", "lutris");
            yield return Path.Combine(home, "star-citizen");
            yield return Path.Combine(home, "StarCitizen");
            yield return Path.Combine(home, "hry");
        }

        yield return "/mnt";
        yield return "/media";
        yield return "/run/media";
        yield return "/home/data";
    }

    private static void ScanDirectory(string currentDir, int depth, HashSet<string> found)
    {
        try
        {
            foreach (var relativeTarget in RelativeTargets)
            {
                var candidate = Path.Combine(currentDir, relativeTarget);
                if (Directory.Exists(candidate))
                {
                    found.Add(Path.GetFullPath(candidate));
                }
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
}
