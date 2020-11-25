using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using GW2EIGW2API.GW2API;
using Newtonsoft.Json;

namespace GW2EIGW2API
{
    internal static class GW2SpecAPIController
    {

        private const string APIPath = "/v2/specializations";

        private static GW2APIUtilities.APIItems<GW2APISpec> _apiSpecs = new GW2APIUtilities.APIItems<GW2APISpec>();

        private static List<GW2APISpec> GetGW2APISpecs()
        {
            Console.WriteLine("Getting specs from API");
            return GW2APIUtilities.GetGW2APIItems<GW2APISpec>(APIPath);
        }

        internal static GW2APIUtilities.APIItems<GW2APISpec> GetAPISpecs(string cachePath)
        {
            if (_apiSpecs.Items.Count == 0)
            {
                SetAPISpecs(cachePath);
            }
            return _apiSpecs;
        }

        internal static void WriteAPISpecsToFile(string filePath)
        {
            FileStream fcreate = File.Open(filePath, FileMode.Create);
            fcreate.Close();

            List<GW2APISpec> specList = GetGW2APISpecs();
            var writer = new StreamWriter(Path.GetDirectoryName(filePath));
            GW2APIUtilities.Serializer.Serialize(writer, specList);
            writer.Close();

            // refresh cache
            _apiSpecs = new GW2APIUtilities.APIItems<GW2APISpec>(specList);
        }

        private static void SetAPISpecs(string filePath)
        {
            if (File.Exists(filePath) && new FileInfo(filePath).Length != 0)
            {
                Console.WriteLine("Reading SpecList");
                using (var reader = new StreamReader(filePath))
                {
                    var specList = (List<GW2APISpec>)GW2APIUtilities.Deserializer.Deserialize(reader, typeof(List<GW2APISpec>));
                    _apiSpecs = new GW2APIUtilities.APIItems<GW2APISpec>(specList);
                    reader.Close();
                }
            }
            else
            {
                _apiSpecs = new GW2APIUtilities.APIItems<GW2APISpec>(GetGW2APISpecs());
            }
        }
    }
}

