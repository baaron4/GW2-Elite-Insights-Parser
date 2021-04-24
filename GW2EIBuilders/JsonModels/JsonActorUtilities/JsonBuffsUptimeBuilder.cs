using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using Newtonsoft.Json;
using static GW2EIJSON.JsonBuffsUptime;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Class representing buff on targets
    /// </summary>
    internal static class JsonBuffsUptimeBuilder
    {
        private static Dictionary<string, double> ConvertKeys(Dictionary<AbstractSingleActor, double> toConvert)
        {
            var res = new Dictionary<string, double>();
            foreach (KeyValuePair<AbstractSingleActor, double> pair in toConvert)
            {
                res[pair.Key.Character] = pair.Value;
            }
            return res;
        }

        public static JsonBuffsUptimeData BuildJsonBuffsUptimeData(FinalBuffs buffs, FinalBuffsDictionary buffsDictionary)
        {
            var jsonBuffsUptimeData = new JsonBuffsUptimeData();
            jsonBuffsUptimeData.Uptime = buffs.Uptime;
            jsonBuffsUptimeData.Presence = buffs.Presence;
            jsonBuffsUptimeData.Generated = ConvertKeys(buffsDictionary.Generated);
            jsonBuffsUptimeData.Overstacked = ConvertKeys(buffsDictionary.Overstacked);
            jsonBuffsUptimeData.Wasted = ConvertKeys(buffsDictionary.Wasted);
            jsonBuffsUptimeData.UnknownExtended = ConvertKeys(buffsDictionary.UnknownExtension);
            jsonBuffsUptimeData.ByExtension = ConvertKeys(buffsDictionary.Extension);
            jsonBuffsUptimeData.Extended = ConvertKeys(buffsDictionary.Extended);
            return jsonBuffsUptimeData;
        }


        public static JsonBuffsUptime BuildJsonBuffsUptime(AbstractSingleActor actor, long buffID, ParsedEvtcLog log, RawFormatSettings settings, List<JsonBuffsUptimeData> buffData, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var jsonBuffsUptime = new JsonBuffsUptime();
            jsonBuffsUptime.Id = buffID;
            jsonBuffsUptime.BuffData = buffData;
            if (!buffDesc.ContainsKey("b" + buffID))
            {
                buffDesc["b" + buffID] = JsonLogBuilder.BuildBuffDesc(log.Buffs.BuffsByIds[buffID], log);
            }
            if (settings.RawFormatTimelineArrays)
            {
                jsonBuffsUptime.States = GetBuffStates(actor.GetBuffGraphs(log)[buffID]);
            }
            return jsonBuffsUptime;
        }
        public static List<int[]> GetBuffStates(BuffsGraphModel bgm)
        {
            if (bgm == null || bgm.BuffChart.Count == 0)
            {
                return null;
            }
            var res = bgm.BuffChart.Select(x => new int[2] { (int)x.Start, (int)x.Value }).ToList();
            return res.Count > 0 ? res : null;
        }
    }

}
