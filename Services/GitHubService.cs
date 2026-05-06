using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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

    public async Task<string?> GetLatestAppVersionAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.GitHubLatestReleaseApi);
            request.Headers.UserAgent.ParseAdd("ScCestinator");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var contentBytes = await response.Content.ReadAsByteArrayAsync();
            var json = JsonDocument.Parse(contentBytes);

            if (json.RootElement.TryGetProperty("tag_name", out var tag))
            {
                var tagValue = tag.GetString();
                if (string.IsNullOrWhiteSpace(tagValue))
                    return null;

                return tagValue.TrimStart('v', 'V');
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
