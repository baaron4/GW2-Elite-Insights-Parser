using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace GW2EIParserAvalonia.Services;

public sealed class SkillIconService
{
    private static readonly HttpClient HttpClient = CreateHttpClient();
    private readonly ConcurrentDictionary<string, Task<Bitmap?>> _cache = new();

    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("GW2EIParser"); // wiki.guildwars2.com wants a user set
        return client;
    }

    public Task<Bitmap?> LoadAsync(Uri uri)
    {
        if (!uri.IsAbsoluteUri)
        {
            return Task.FromResult<Bitmap?>(null);
        }

        return _cache.GetOrAdd(uri.AbsoluteUri, _ => LoadBitmapAsync(uri));
    }

    private static async Task<Bitmap?> LoadBitmapAsync(Uri uri)
    {
        try
        {
            var bytes = await HttpClient.GetByteArrayAsync(uri);
            await using var stream = new MemoryStream(bytes);
            return new Bitmap(stream);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
