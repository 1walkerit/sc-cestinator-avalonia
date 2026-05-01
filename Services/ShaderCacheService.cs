using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScCestinator.Services;

public class ShaderCacheService
{
    public string? FindWinePrefix(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        var dir = new DirectoryInfo(path);

        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "drive_c")))
                return dir.FullName;

            dir = dir.Parent;
        }

        return null;
    }

    public List<string> GetShaderPaths(string? inputPath)
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var paths = new List<string>
        {
            Path.Combine(home, ".cache", "mesa_shader_cache"),
            Path.Combine(home, ".cache", "nvidia"),
            Path.Combine(home, ".nv", "GLCache")
        };

        var winePrefix = FindWinePrefix(inputPath);

        if (!string.IsNullOrWhiteSpace(winePrefix) && Directory.Exists(winePrefix))
        {
            paths.Add(Path.Combine(winePrefix, "mesa_shader_cache"));
            paths.Add(Path.Combine(winePrefix, "GLCache"));
            paths.Add(Path.Combine(winePrefix, "radv_builtin_shaders"));
        }

        return paths;
    }
    
    public List<string> GetExistingShaderPaths(string? inputPath)
{
    return GetShaderPaths(inputPath)
        .Where(Directory.Exists)
        .ToList();
}
    public int ClearShaders(string? inputPath)
    {
        var paths = GetShaderPaths(inputPath);

        int totalDeleted = 0;

        foreach (var dir in paths)
        {
            if (!Directory.Exists(dir))
                continue;

            var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                    totalDeleted++;
                }
                catch
                {
                    // ignorujeme chyby
                }
            }
        }

        return totalDeleted;
    }
}
