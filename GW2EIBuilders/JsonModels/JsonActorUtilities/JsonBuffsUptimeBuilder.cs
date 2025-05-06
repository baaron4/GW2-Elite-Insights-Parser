using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using static GW2EIJSON.JsonBuffsUptime;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities;

/// <summary>
/// Class representing buff on targets
/// </summary>
internal static class JsonBuffsUptimeBuilder
{
    private static Dictionary<string, double> ConvertKeys(IReadOnlyDictionary<SingleActor, double> toConvert)
    {
        var res = new Dictionary<string, double>(toConvert.Count);
        foreach (KeyValuePair<SingleActor, double> pair in toConvert)
        {
            res[pair.Key.Character] = pair.Value;
        }
        return res;
    }

    public static JsonBuffsUptimeData BuildJsonBuffsUptimeData(BuffStatistics buffs, BuffByActorStatistics buffsDictionary)
    {
        var jsonBuffsUptimeData = new JsonBuffsUptimeData
        {
            Uptime = buffs.Uptime,
            Presence = buffs.Presence,
            Generated = ConvertKeys(buffsDictionary.GeneratedBy),
            Overstacked = ConvertKeys(buffsDictionary.OverstackedBy),
            Wasted = ConvertKeys(buffsDictionary.WastedFrom),
            UnknownExtended = ConvertKeys(buffsDictionary.UnknownExtensionFrom),
            ByExtension = ConvertKeys(buffsDictionary.ExtensionBy),
            Extended = ConvertKeys(buffsDictionary.ExtendedFrom)
        };
        return jsonBuffsUptimeData;
    }


    public static JsonBuffsUptime BuildJsonBuffsUptime(SingleActor actor, long buffID, ParsedEvtcLog log, RawFormatSettings settings, List<JsonBuffsUptimeData> buffData, Dictionary<long, Buff> buffMap)
    {
        var jsonBuffsUptime = new JsonBuffsUptime
        {
            Id = buffID,
            BuffData = buffData
        };
        if (!buffMap.ContainsKey(buffID))
        {
            buffMap[buffID] = log.Buffs.BuffsByIds[buffID];
        }
        if (settings.RawFormatTimelineArrays)
        {
            jsonBuffsUptime.States = GetBuffStates(actor.GetBuffGraphs(log)[buffID]).ToList();
            IReadOnlyDictionary<long, BuffByActorStatistics> buffDicts = actor.GetBuffsDictionary(log, log.FightData.FightStart, log.FightData.FightEnd);
            if (buffDicts.TryGetValue(buffID, out var buffDict))
            {
                var statesPerSource = new Dictionary<string, IReadOnlyList<IReadOnlyList<long>>>(buffDict.GeneratedBy.Count);
                foreach (SingleActor source in buffDict.GeneratedBy.Keys)
                {
                    statesPerSource[source.Character] = GetBuffStates(actor.GetBuffGraphs(log, source)[buffID]).ToList();
                }
                jsonBuffsUptime.StatesPerSource = statesPerSource;
            }
        }
        return jsonBuffsUptime;
    }
    public static IEnumerable<IReadOnlyList<long>> GetBuffStates(BuffGraph? bgm)
    {
        if (bgm == null || bgm.Values.Count == 0)
        {
            return [];
        }

        return bgm.Values.Select(x => new List<long>() { x.Start, (int)x.Value });
    }
}
