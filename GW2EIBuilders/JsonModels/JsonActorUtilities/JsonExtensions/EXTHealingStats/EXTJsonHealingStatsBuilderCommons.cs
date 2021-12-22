using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using Newtonsoft.Json;

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

        private static EXTJsonHealingDist BuildHealingDist(long id, List<EXTAbstractHealingEvent> list, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var jsonHealingDist = new EXTJsonHealingDist();
            jsonHealingDist.IndirectHealing = list.Exists(x => x is EXTNonDirectHealingEvent);
            if (jsonHealingDist.IndirectHealing)
            {
                if (!buffDesc.ContainsKey("b" + id))
                {
                    if (log.Buffs.BuffsByIds.TryGetValue(id, out Buff buff))
                    {
                        buffDesc["b" + id] = JsonLogBuilder.BuildBuffDesc(buff, log);
                    }
                    else
                    {
                        SkillItem skill = list.First().Skill;
                        var auxBoon = new Buff(skill.Name, id, skill.Icon);
                        buffDesc["b" + id] = JsonLogBuilder.BuildBuffDesc(auxBoon, log);
                    }
                }
            }
            else
            {
                if (!skillDesc.ContainsKey("s" + id))
                {
                    SkillItem skill = list.First().Skill;
                    skillDesc["s" + id] = JsonLogBuilder.BuildSkillDesc(skill, log);
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

        internal static List<EXTJsonHealingDist> BuildHealingDistList(Dictionary<long, List<EXTAbstractHealingEvent>> dlsByID, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<EXTJsonHealingDist>();
            foreach (KeyValuePair<long, List<EXTAbstractHealingEvent>> pair in dlsByID)
            {
                res.Add(BuildHealingDist(pair.Key, pair.Value, log, skillDesc, buffDesc));
            }
            return res;
        }
    }
}
