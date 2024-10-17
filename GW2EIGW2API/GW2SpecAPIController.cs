using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GW2EIGW2API.GW2API;

namespace GW2EIGW2API;

internal class GW2SpecAPIController
{

    private const string APIPath = "/v2/specializations";

    private GW2APIUtilities.APIItems<GW2APISpec> _apiSpecs = new GW2APIUtilities.APIItems<GW2APISpec>();

    private static List<GW2APISpec> GetGW2APISpecs()
    {
        Console.WriteLine("Getting specs from API");
        return GW2APIUtilities.GetGW2APIItems<GW2APISpec>(APIPath);
    }

    internal GW2APIUtilities.APIItems<GW2APISpec> GetAPISpecs(string cachePath)
    {
        if (_apiSpecs.Items.Count == 0)
        {
            SetAPISpecs(cachePath);
        }
        return _apiSpecs;
    }

    internal void WriteAPISpecsToFile(string filePath)
    {
        FileStream fcreate = File.Open(filePath, FileMode.Create);
        fcreate.Close();

        List<GW2APISpec> specList = GetGW2APISpecs();
        using(var writer = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
            JsonSerializer.Serialize(writer, specList, GW2APIUtilities.SerializerSettings);
        }

        // refresh cache
        _apiSpecs = new GW2APIUtilities.APIItems<GW2APISpec>(specList);
    }

    private void SetAPISpecs(string filePath)
    {
        var fi = new FileInfo(filePath);
        if (fi.Exists && fi.Length != 0)
        {
            Console.WriteLine("Reading SpecList");
            using (var reader = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var specList = JsonSerializer.Deserialize<List<GW2APISpec>>(reader, GW2APIUtilities.DeserializerSettings);
                _apiSpecs = new GW2APIUtilities.APIItems<GW2APISpec>(specList);
            }
        }
        else
        {
            _apiSpecs = new GW2APIUtilities.APIItems<GW2APISpec>(GetGW2APISpecs());
        }
    }
}

