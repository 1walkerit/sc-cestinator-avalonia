namespace ScCestinator.Services;

public static class VersionParser
{
    public static string? ParseVersionFromFirstLine(string? firstLine)
    {
        if (string.IsNullOrWhiteSpace(firstLine))
            return null;

        return firstLine.Trim().TrimStart(';').Trim();
    }
}
