using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GW2EIGW2API.GW2API;

namespace GW2EIGW2API;

internal class GW2TraitAPIController
{
    private const string APIPath = "/v2/traits";


    private GW2APIUtilities.APIItems<GW2APITrait> _apiTraits = new();
    private static List<GW2APITrait> GetGW2APITraits()
    {
        Console.WriteLine("Getting traits from API");
        return GW2APIUtilities.GetGW2APIItems<GW2APITrait>(APIPath);
    }

    internal GW2APIUtilities.APIItems<GW2APITrait> GetAPITraits(string cachePath)
    {
        if (_apiTraits.Items.Count == 0)
        {
            SetAPITraits(cachePath);
        }
        return _apiTraits;
    }

    internal void WriteAPITraitsToFile(string filePath)
    {
        FileStream fcreate = File.Open(filePath, FileMode.Create);
        fcreate.Close();

        List<GW2APITrait> traitList = GetGW2APITraits();
        using(var writer = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
            JsonSerializer.Serialize(writer, traitList, GW2APIUtilities.SerializerSettings);
        }

        // refresh API cache
        _apiTraits = new GW2APIUtilities.APIItems<GW2APITrait>(traitList);
    }
    private void SetAPITraits(string filePath)
    {
        var fi = new FileInfo(filePath);
        if (fi.Exists && fi.Length != 0)
        {
            Console.WriteLine("Reading Traitlist");
            using (var reader = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var traitList = JsonSerializer.Deserialize<List<GW2APITrait>>(reader, GW2APIUtilities.DeserializerSettings);
                _apiTraits = new GW2APIUtilities.APIItems<GW2APITrait>(traitList);
            }
        }
        else
        {
            _apiTraits = new GW2APIUtilities.APIItems<GW2APITrait>(GetGW2APITraits());
        }
    }
}

