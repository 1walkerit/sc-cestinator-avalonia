using System;
using System.Net.Http;

namespace ScCestinator.Services;

public static class HttpClientFactory
{
    private static readonly HttpClient _sharedClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    public static HttpClient GetSharedClient() => _sharedClient;
}
