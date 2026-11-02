using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIBuilders.JsonModels.JsonLogBuilder;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTHealing;

internal static class EXTJsonHealingStatsBuilderCommons
{
    internal static EXTJsonHealingStatistics.EXTJsonOutgoingHealingStatistics BuildOutgoingHealingStatistics(EXTFinalOutgoingHealingStat stats)
    {
        return new EXTJsonHealingStatistics.EXTJsonOutgoingHealingStatistics()
        {
            ConversionHealing = stats.ConversionHealing,
            ConversionHps = stats.ConversionHps,
            Healing = stats.Healing,
            HealingPowerHealing = stats.HealingPowerHealing,
            HealingPowerHps = stats.HealingPowerHps,
            Hps = stats.Hps,
            HybridHealing = stats.HybridHealing,
            HybridHps = stats.HybridHps,
            DownedHealing = stats.DownedHealing,
            DownedHps = stats.DownedHps,

            ActorConversionHealing = stats.ActorConversionHealing,
            ActorConversionHps = stats.ActorConversionHps,
            ActorHealing = stats.ActorHealing,
            ActorHealingPowerHealing = stats.ActorHealingPowerHealing,
            ActorHealingPowerHps = stats.ActorHealingPowerHps,
            ActorHps = stats.ActorHps,
            ActorHybridHealing = stats.ActorHybridHealing,
            ActorHybridHps = stats.ActorHybridHps,
            ActorDownedHealing = stats.ActorDownedHealing,
            ActorDownedHps = stats.ActorDownedHps,
        };
    }

    internal static EXTJsonHealingStatistics.EXTJsonIncomingHealingStatistics BuildIncomingHealingStatistics(EXTFinalIncomingHealingStat stats)
    {
        return new EXTJsonHealingStatistics.EXTJsonIncomingHealingStatistics()
        {
            ConversionHealed = stats.ConversionHealed,
            Healed = stats.Healed,
            HealingPowerHealed = stats.HealingPowerHealed,
            HybridHealed = stats.HybridHealed,
            DownedHealed = stats.DownedHealed,
        };
    }

    //TODO_PERF(Rennorb)
    private static EXTJsonHealingDist BuildHealingDist(long id, List<EXTHealingEvent> list, ParsedEvtcLog log, UsedJsonData usedData)
    {
        var jsonHealingDist = new EXTJsonHealingDist
        {
            IndirectHealing = list.Exists(x => x is EXTNonDirectHealingEvent)
        };
        if (jsonHealingDist.IndirectHealing)
        {
            if (!usedData.buffs.ContainsKey(id))
            {
                if (log.Buffs.BuffsByIDs.TryGetValue(id, out var buff))
                {
                    usedData.buffs[id] = buff;
                }
                else
                {
                    SkillItem skill = list.First().Skill;
                    var auxBuff = new Buff(skill.Name, id, skill.Icon);
                    usedData.buffs[id] = auxBuff;
                }
            }
        }
        else
        {
            usedData.skills.TryAdd(id, list.First().Skill);
        }
        jsonHealingDist.Id = id;
        jsonHealingDist.Min = int.MaxValue;
        jsonHealingDist.Max = int.MinValue;
        foreach (EXTHealingEvent healingEvt in list)
        {
            jsonHealingDist.Hits++; ;
            jsonHealingDist.TotalHealing += healingEvt.HealingDone;
            if (healingEvt.AgainstDowned)
            {
                jsonHealingDist.TotalDownedHealing += healingEvt.HealingDone;
            }
            jsonHealingDist.Min = Math.Min(jsonHealingDist.Min, healingEvt.HealingDone);
            jsonHealingDist.Max = Math.Max(jsonHealingDist.Max, healingEvt.HealingDone);
        }
        jsonHealingDist.Min = jsonHealingDist.Min == int.MaxValue ? 0 : jsonHealingDist.Min;
        jsonHealingDist.Max = jsonHealingDist.Max == int.MinValue ? 0 : jsonHealingDist.Max;
        return jsonHealingDist;
    }

    //TODO_PERF(Rennorb)
    internal static List<EXTJsonHealingDist> BuildHealingDistList(IEnumerable<IGrouping<long, EXTHealingEvent>> dlsByID, ParsedEvtcLog log, UsedJsonData usedData)
    {
        var res = new List<EXTJsonHealingDist>();
        foreach (var pair in dlsByID)
        {
            res.Add(BuildHealingDist(pair.Key, pair.ToList(), log, usedData));
        }
        return res;
    }
}
