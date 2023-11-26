using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using Newtonsoft.Json;
using static GW2EIJSON.JsonBuffsUptime;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities
{
    /// <summary>
    /// Class representing buff on targets
    /// </summary>
    internal static class JsonBuffsUptimeBuilder
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

        public static JsonBuffsUptimeData BuildJsonBuffsUptimeData(FinalActorBuffs buffs, FinalBuffsDictionary buffsDictionary)
        {
            var jsonBuffsUptimeData = new JsonBuffsUptimeData
            {
                Uptime = buffs.Uptime,
                Presence = buffs.Presence,
                Generated = ConvertKeys(buffsDictionary.Generated),
                Overstacked = ConvertKeys(buffsDictionary.Overstacked),
                Wasted = ConvertKeys(buffsDictionary.Wasted),
                UnknownExtended = ConvertKeys(buffsDictionary.UnknownExtension),
                ByExtension = ConvertKeys(buffsDictionary.Extension),
                Extended = ConvertKeys(buffsDictionary.Extended)
            };
            return jsonBuffsUptimeData;
        }


        public static JsonBuffsUptime BuildJsonBuffsUptime(AbstractSingleActor actor, long buffID, ParsedEvtcLog log, RawFormatSettings settings, List<JsonBuffsUptimeData> buffData, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var jsonBuffsUptime = new JsonBuffsUptime
            {
                Id = buffID,
                BuffData = buffData
            };
            if (!buffDesc.ContainsKey("b" + buffID))
            {
                buffDesc["b" + buffID] = JsonLogBuilder.BuildBuffDesc(log.Buffs.BuffsByIds[buffID], log);
            }
            if (settings.RawFormatTimelineArrays)
            {
                jsonBuffsUptime.States = GetBuffStates(actor.GetBuffGraphs(log)[buffID]);
                IReadOnlyDictionary<long, FinalBuffsDictionary> buffDicts = actor.GetBuffsDictionary(log, log.FightData.FightStart, log.FightData.FightEnd);
                if (buffDicts.TryGetValue(buffID, out FinalBuffsDictionary buffDict))
                {
                    var statesPerSource = new Dictionary<string, IReadOnlyList<IReadOnlyList<int>>>();
                    foreach (AbstractSingleActor source in buffDict.Generated.Keys)
                    {
                        statesPerSource[source.Character] = GetBuffStates(actor.GetBuffGraphs(log, source)[buffID]);
                    }
                    jsonBuffsUptime.StatesPerSource = statesPerSource;
                }
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
