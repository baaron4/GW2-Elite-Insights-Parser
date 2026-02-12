using GW2EIBuilders.JsonModels.JsonActorUtilities;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIJSON.JsonBuffsUptime;
using static GW2EIJSON.JsonBuffVolumes;

namespace GW2EIBuilders.JsonModels.JsonActors;

/// <summary>
/// Class representing an NPC
/// </summary>
internal static class JsonNPCBuilder
{

    public static JsonNPC BuildJsonNPC(SingleActor npc, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap, HashSet<ulong> teampMap)
    {
        var jsonNPC = new JsonNPC();
        JsonActorBuilder.FillJsonActor(jsonNPC, npc, log, settings, skillMap, buffMap, teampMap);
        //
        jsonNPC.Id = npc.ID;
        jsonNPC.EnemyPlayer = npc is PlayerNonSquad;
        double hpLeft = 100.0;
        double barrierLeft = 0.0;
        var targetEncounterPhase = log.LogData.GetEncounterPhases(log).FirstOrDefault(x => x.Targets.ContainsKey(npc));
        if (targetEncounterPhase != null && targetEncounterPhase.Success)
        {
            hpLeft = 0;
        }
        else
        {
            var hpUpdates = npc.GetHealthUpdates(log);
            if (hpUpdates.Count > 0)
            {
                hpLeft = hpUpdates.Last().Value;
            }
            var barrierUpdates = npc.GetBarrierUpdates(log);
            if (barrierUpdates.Count > 0)
            {
                barrierLeft = barrierUpdates.Last().Value;
            }
        }
        jsonNPC.HealthPercentBurned = 100.0 - hpLeft;
        jsonNPC.BarrierPercent = barrierLeft;
        jsonNPC.FinalHealth = npc.GetCurrentHealth(log, hpLeft);
        jsonNPC.FinalBarrier = npc.GetCurrentBarrier(log, barrierLeft, log.LogData.LogEnd);
        //
        jsonNPC.Buffs = GetNPCJsonBuffsUptime(npc, log, settings, buffMap);
        jsonNPC.BuffVolumes = GetNPCJsonBuffVolumes(npc, log, buffMap);
        // Breakbar
        if (settings.RawFormatTimelineArrays)
        {
            jsonNPC.BreakbarPercents = npc.GetBreakbarPercentUpdates(log).Select(x => new List<double>() { x.Start, x.Value }).ToList();
        }
        return jsonNPC;
    }

    private static List<JsonBuffsUptime> GetNPCJsonBuffsUptime(SingleActor npc, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<long, Buff> buffMap)
    {
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        var buffs = phases.Select(x => npc.GetBuffs(ParserHelper.BuffEnum.Self, log, x.Start, x.End)).ToList();
        var res = new List<JsonBuffsUptime>(buffs[0].Count);
        var buffDictionaries = phases.Select(x => npc.GetBuffsDictionary(log, x.Start, x.End)).ToList();
        foreach (KeyValuePair<long, BuffStatistics> pair in buffs[0])
        {
            Buff buff = log.Buffs.BuffsByIDs[pair.Key];
            if (buff.Classification == Buff.BuffClassification.Hidden)
            {
                continue;
            }
            var data = new List<JsonBuffsUptimeData>(phases.Count);
            for (int i = 0; i < phases.Count; i++)
            {
                if (buffs[i].TryGetValue(pair.Key, out var buffstats) && buffDictionaries[i].TryGetValue(pair.Key, out var buffsDict))
                {
                    JsonBuffsUptimeData value = JsonBuffsUptimeBuilder.BuildJsonBuffsUptimeData(buffstats, buffsDict);
                    data.Add(value);
                }
                else
                {
                    var value = new JsonBuffsUptimeData();
                    data.Add(value);
                }
            }
            res.Add(JsonBuffsUptimeBuilder.BuildJsonBuffsUptime(npc, pair.Key, log, settings, data, buffMap));
        }
        return res;
    }
    private static List<JsonBuffVolumes> GetNPCJsonBuffVolumes(SingleActor npc, ParsedEvtcLog log, Dictionary<long, Buff> buffMap)
    {
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        var buffVolumes = phases.Select(x => npc.GetBuffVolumes(ParserHelper.BuffEnum.Self, log, x.Start, x.End)).ToList();
        var res = new List<JsonBuffVolumes>(buffVolumes[0].Count);
        var buffVolumeDictionaries = phases.Select(x => npc.GetBuffVolumesDictionary(log, x.Start, x.End)).ToList();
        foreach (KeyValuePair<long, BuffVolumeStatistics> pair in buffVolumes[0])
        {
            Buff buff = log.Buffs.BuffsByIDs[pair.Key];
            if (buff.Classification == Buff.BuffClassification.Hidden)
            {
                continue;
            }
            var data = new List<JsonBuffVolumesData>(phases.Count);
            for (int i = 0; i < phases.Count; i++)
            {
                if (buffVolumes[i].TryGetValue(pair.Key, out var buffStats) && buffVolumeDictionaries[i].TryGetValue(pair.Key, out var buffDicts))
                {
                    JsonBuffVolumesData value = JsonBuffVolumesBuilder.BuildJsonBuffVolumesData(buffStats, buffDicts);
                    data.Add(value);
                }
                else
                {
                    var value = new JsonBuffVolumesData();
                    data.Add(value);
                }
            }
            res.Add(JsonBuffVolumesBuilder.BuildJsonBuffVolumes(pair.Key, log, data, buffMap));
        }
        return res;
    }
}
