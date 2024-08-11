using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTHealing
{
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

        private static EXTJsonHealingDist BuildHealingDist(long id, List<EXTAbstractHealingEvent> list, ParsedEvtcLog log, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
        {
            var jsonHealingDist = new EXTJsonHealingDist();
            jsonHealingDist.IndirectHealing = list.Exists(x => x is EXTNonDirectHealingEvent);
            if (jsonHealingDist.IndirectHealing)
            {
                if (!buffMap.ContainsKey(id))
                {
                    if (log.Buffs.BuffsByIds.TryGetValue(id, out Buff buff))
                    {
                        buffMap[id] = buff;
                    }
                    else
                    {
                        SkillItem skill = list.First().Skill;
                        var auxBuff = new Buff(skill.Name, id, skill.Icon);
                        buffMap[id] = auxBuff;
                    }
                }
            }
            else
            {
                if (!skillMap.ContainsKey(id))
                {
                    SkillItem skill = list.First().Skill;
                    skillMap[id] = skill;
                }
            }
            jsonHealingDist.Id = id;
            jsonHealingDist.Min = int.MaxValue;
            jsonHealingDist.Max = int.MinValue;
            foreach (EXTAbstractHealingEvent healingEvt in list)
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

        internal static List<EXTJsonHealingDist> BuildHealingDistList(Dictionary<long, List<EXTAbstractHealingEvent>> dlsByID, ParsedEvtcLog log, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
        {
            var res = new List<EXTJsonHealingDist>();
            foreach (KeyValuePair<long, List<EXTAbstractHealingEvent>> pair in dlsByID)
            {
                res.Add(BuildHealingDist(pair.Key, pair.Value, log, skillMap, buffMap));
            }
            return res;
        }
    }
}
