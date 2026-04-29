using System.Text.RegularExpressions;

namespace ScCestinator.Services;

public static class VersionParser
{
    // Matches versions like: 0.9y, 0.10, 1.0, 1.2a
    private static readonly Regex VersionRegex = new(@"\d+\.\d+[a-zA-Z]?", RegexOptions.Compiled);

    public static string? ParseVersionFromFirstLine(string? firstLine)
    {
        if (string.IsNullOrWhiteSpace(firstLine))
            return null;

        var match = VersionRegex.Match(firstLine);
        return match.Success ? match.Value : null;
    }
}
