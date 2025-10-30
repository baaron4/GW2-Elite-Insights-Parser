using System.Text.Json;
using GW2EIGW2API.GW2API;

namespace GW2EIGW2API;

internal static class GW2APIUtilities
{
    internal static readonly JsonSerializerOptions SerializerSettings = new()
    {
        WriteIndented = false,
        IncludeFields = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        //NOTE(Rennorb): does html escape by default
    };

    internal static readonly JsonSerializerOptions DeserializerSettings = new()
    {
        IncludeFields = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        //NOTE(Rennorb): does html escape by default
    };

    // utilities
    internal class APIItems<T> where T : GW2APIBaseItem
    {
        public APIItems()
        {
            Items = new Dictionary<long, T>();
        }

        public APIItems(List<T> traits)
        {
            Items = traits.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.FirstOrDefault());
        }

        public readonly Dictionary<long, T> Items;
    }

    internal static List<T> GetGW2APIItems<T>(string apiPath) where T : GW2APIBaseItem
    {
        var itemList = new List<T>();
        bool maxPageSizeReached = false;
        int page = 0;
        int pagesize = 200;
        while (!maxPageSizeReached)
        {
            string path = apiPath + "?page=" + page + "&page_size=" + pagesize + "&lang=en";
            HttpResponseMessage response = GetAPIClient().GetAsync(new Uri(path, UriKind.Relative)).Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsByteArrayAsync().Result;
                var json = JsonDocument.Parse(data); //TODO_PERF(Rennorb): use utf8reader in a loop for each object and avoid the array allocation
                T[] responseArray = JsonSerializer.Deserialize<T[]>(json, DeserializerSettings);
                itemList.AddRange(responseArray);
            }
            else
            {
                maxPageSizeReached = true;
            }
            page++;
        }

        return itemList;
    }
    // 

    private static HttpClient APIClient;

    internal static HttpClient GetAPIClient()
    {
        if (APIClient == null)
        {
            APIClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.guildwars2.com")
            };
            APIClient.DefaultRequestHeaders.Accept.Clear();
            APIClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        return APIClient;
    }
}

