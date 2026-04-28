using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScCestinator.Services;

public sealed class GitHubService
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    public async Task<string?> GetOnlineVersionAsync()
    {
        try
        {
            var content = await _httpClient.GetStringAsync(Constants.GitHubLocalizationUrl);

            using var reader = new StringReader(content);
            var firstLine = reader.ReadLine();

            return VersionParser.ParseVersionFromFirstLine(firstLine);
        }
        catch
        {
            return null;
        }
    }
}
