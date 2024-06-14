using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using Newtonsoft.Json;
using static GW2EIJSON.JsonBuffVolumes;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities
{
    /// <summary>
    /// Class representing buff on targets
    /// </summary>
    internal static class JsonBuffVolumesBuilder
    {
        private static Dictionary<string, double> ConvertKeys(IReadOnlyDictionary<AbstractSingleActor, double> toConvert)
        {
            var res = new Dictionary<string, double>();
            foreach (KeyValuePair<AbstractSingleActor, double> pair in toConvert)
            {
                res[pair.Key.Character] = pair.Value;
            }
            return res;
        }

        public static JsonBuffVolumesData BuildJsonBuffVolumesData(FinalActorBuffVolumes buffs, FinalBuffVolumesDictionary buffsDictionary)
        {
            var jsonBuffsUptimeData = new JsonBuffVolumesData
            {
                Incoming = buffs.Incoming,
                IncomingByExtension = buffs.IncomingByExtension,
                IncomingByUnknownExtension = buffs.IncomingByUnknownExtension,
                IncomingBy = ConvertKeys(buffsDictionary.IncomingBy),
                IncomingByExtensionBy = ConvertKeys(buffsDictionary.IncomingByExtensionBy),
            };
            return jsonBuffsUptimeData;
        }


        public static JsonBuffVolumes BuildJsonBuffVolumes(long buffID, ParsedEvtcLog log, List<JsonBuffVolumesData> buffVolumeData, Dictionary<long, Buff> buffMap)
        {
            var jsonBuffVolumes = new JsonBuffVolumes
            {
                Id = buffID,
                BuffVolumeData = buffVolumeData
            };
            if (!buffMap.ContainsKey(buffID))
            {
                buffMap[buffID] = log.Buffs.BuffsByIds[buffID];
            }
            return jsonBuffVolumes;
        }
    }

}
