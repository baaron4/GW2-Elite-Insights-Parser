using System.Text.Json;
using GW2EIGW2API.GW2API;

namespace GW2EIGW2API;

internal class GW2MapAPIController
{

    private const string APIPath = "/v2/maps";

    private GW2APIUtilities.APIItems<GW2APIMap> _apiMaps = new();

    private static List<GW2APIMap> GetGW2APIMaps()
    {
        Console.WriteLine("Getting maps from API");
        return GW2APIUtilities.GetGW2APIItems<GW2APIMap>(APIPath);
    }

    internal GW2APIUtilities.APIItems<GW2APIMap> GetAPIMaps(string cachePath)
    {
        if (_apiMaps.Items.Count == 0)
        {
            SetAPIMaps(cachePath);
        }
        return _apiMaps;
    }

    internal void WriteAPIMapsToFile(string filePath)
    {
        FileStream fcreate = File.Open(filePath, FileMode.Create);
        fcreate.Close();

        var mapList = GetGW2APIMaps();
        using(var writer = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
            JsonSerializer.Serialize(writer, mapList, GW2APIUtilities.SerializerSettings);
        }

        // refresh cache
        _apiMaps = new GW2APIUtilities.APIItems<GW2APIMap>(mapList);
    }

    private void SetAPIMaps(string filePath)
    {
        var fi = new FileInfo(filePath);
        if (fi.Exists && fi.Length != 0)
        {
            Console.WriteLine("Reading MapList");
            using (var reader = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var mapList = JsonSerializer.Deserialize<List<GW2APIMap>>(reader, GW2APIUtilities.DeserializerSettings);
                _apiMaps = new GW2APIUtilities.APIItems<GW2APIMap>(mapList);
            }
        }
        else
        {
            _apiMaps = new GW2APIUtilities.APIItems<GW2APIMap>(GetGW2APIMaps());
        }
    }
}

