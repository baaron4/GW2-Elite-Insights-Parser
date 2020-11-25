using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using GW2EIGW2API.GW2API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GW2EIGW2API
{
    internal static class GW2APIUtilities
    {
        internal static readonly DefaultContractResolver DefaultJsonContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        internal static readonly JsonSerializer Serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = DefaultJsonContractResolver
        };

        internal static readonly JsonSerializer Deserializer = new JsonSerializer
        {
            ContractResolver = DefaultJsonContractResolver
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
                Items = traits.ToDictionary(x => x.Id);
            }

            public Dictionary<long, T> Items { get; }
        }

        internal static List<T> GetGW2APIItems<T>(string apiPath) where T : GW2APIBaseItem
        {
            var itemList = new List<T>();
            bool maxPageSizeReached = false;
            int page = 0;
            int pagesize = 200;
            while (!maxPageSizeReached)
            {
                string path = apiPath + "?page=" + page + "&page_size=" + pagesize;
                HttpResponseMessage response = GetAPIClient().GetAsync(new Uri(path, UriKind.Relative)).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    T[] responseArray = JsonConvert.DeserializeObject<T[]>(data, new JsonSerializerSettings
                    {
                        ContractResolver = DefaultJsonContractResolver
                    });
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

        private static HttpClient APIClient { get; set; }

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
}

