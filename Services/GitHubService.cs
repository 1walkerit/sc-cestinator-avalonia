using System;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScCestinator.Services;

public sealed class GitHubService
{
    private static readonly HttpClient _httpClient = HttpClientFactory.GetSharedClient();

    public async Task<string?> GetOnlineVersionAsync()
    {
        try
        {
            var contentBytes = await _httpClient.GetByteArrayAsync(Constants.GitHubLocalizationUrl);
            var content = Encoding.UTF8.GetString(contentBytes);

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
