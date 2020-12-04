using System;
using System.Collections.Generic;
using System.IO;
using GW2EIGW2API.GW2API;

namespace GW2EIGW2API
{
    internal class GW2TraitAPIController
    {
        private const string APIPath = "/v2/traits";


        private GW2APIUtilities.APIItems<GW2APITrait> _apiTraits = new GW2APIUtilities.APIItems<GW2APITrait>();
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
            var writer = new StreamWriter(filePath);
            GW2APIUtilities.Serializer.Serialize(writer, traitList);
            writer.Close();

            // refresh API cache
            _apiTraits = new GW2APIUtilities.APIItems<GW2APITrait>(traitList);
        }
        private void SetAPITraits(string filePath)
        {
            if (File.Exists(filePath) && new FileInfo(filePath).Length != 0)
            {
                Console.WriteLine("Reading Traitlist");
                using (var reader = new StreamReader(filePath))
                {
                    var traitList = (List<GW2APITrait>)GW2APIUtilities.Deserializer.Deserialize(reader, typeof(List<GW2APITrait>));
                    _apiTraits = new GW2APIUtilities.APIItems<GW2APITrait>(traitList);
                    reader.Close();
                }
            }
            else
            {
                _apiTraits = new GW2APIUtilities.APIItems<GW2APITrait>(GetGW2APITraits());
            }
        }
    }
}

