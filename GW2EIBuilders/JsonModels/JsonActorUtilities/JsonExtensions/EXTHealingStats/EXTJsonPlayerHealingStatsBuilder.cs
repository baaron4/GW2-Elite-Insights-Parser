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
    internal static class EXTJsonPlayerHealingStatsBuilder
    {

        public static EXTJsonPlayerHealingStats BuildPlayerHealingStats(AbstractSingleActor a, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var outgoingHealingAllies = new List<List<EXTJsonHealingStatistics.EXTJsonOutgoingHealingStatistics>>();
            var outgoingHealing = new List<EXTJsonHealingStatistics.EXTJsonOutgoingHealingStatistics>();
            var incomingHealing = new List<EXTJsonHealingStatistics.EXTJsonIncomingHealingStatistics>();
            var alliedHealing1S = new List<List<IReadOnlyList<int>>>();
            var alliedHealingPowerHealing1S = new List<List<IReadOnlyList<int>>>();
            var alliedConversionHealingHealing1S = new List<List<IReadOnlyList<int>>>();
            var alliedHybridHealing1S = new List<List<IReadOnlyList<int>>>();
            var healing1S = new List<IReadOnlyList<int>>();
            var healingPowerHealing1S = new List<IReadOnlyList<int>>();
            var conversionHealingHealing1S = new List<IReadOnlyList<int>>();
            var hybridHealing1S = new List<IReadOnlyList<int>>();
            var alliedHealingDist = new List<List<List<EXTJsonHealingDist>>>();
            var totalHealingDist = new List<List<EXTJsonHealingDist>>();
            var totalIncomingHealingDist = new List<List<EXTJsonHealingDist>>();
            var res = new EXTJsonPlayerHealingStats()
            {
                OutgoingHealing = outgoingHealing,
                OutgoingHealingAllies = outgoingHealingAllies,
                IncomingHealing = incomingHealing,
                AlliedHealing1S = alliedHealing1S,
                AlliedConversionHealingHealing1S = alliedConversionHealingHealing1S,
                AlliedHealingPowerHealing1S = alliedHealingPowerHealing1S,
                AlliedHybridHealing1S = alliedHybridHealing1S,
                Healing1S = healing1S,
                HealingPowerHealing1S = healingPowerHealing1S,
                ConversionHealingHealing1S = conversionHealingHealing1S,
                HybridHealing1S = hybridHealing1S,
                AlliedHealingDist = alliedHealingDist,
                TotalHealingDist = totalHealingDist,
                TotalIncomingHealingDist = totalIncomingHealingDist
            };
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (AbstractSingleActor friendly in log.Friendlies)
            {
                //
                var outgoingHealingAlly = new List<EXTJsonHealingStatistics.EXTJsonOutgoingHealingStatistics>();
                outgoingHealingAllies.Add(outgoingHealingAlly);
                //
                var allyHealing1S = new List<IReadOnlyList<int>>();
                alliedHealing1S.Add(allyHealing1S);
                var allyHealingPowerHealing1S = new List<IReadOnlyList<int>>();
                alliedHealingPowerHealing1S.Add(allyHealingPowerHealing1S);
                var allyConversionHealingHealing1S = new List<IReadOnlyList<int>>();
                alliedConversionHealingHealing1S.Add(allyConversionHealingHealing1S);
                var allyHybridHealing1S = new List<IReadOnlyList<int>>();
                alliedHybridHealing1S.Add(allyHybridHealing1S);
                //
                var allyHealingDist = new List<List<EXTJsonHealingDist>>();
                alliedHealingDist.Add(allyHealingDist);
                foreach (PhaseData phase in phases)
                {
                    outgoingHealingAlly.Add(EXTJsonHealingStatsBuilderCommons.BuildOutgoingHealingStatistics(a.EXTHealing.GetOutgoingHealStats(friendly, log, phase.Start, phase.End)));
                    if (settings.RawFormatTimelineArrays)
                    {
                        allyHealing1S.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, friendly, HealingStatsExtensionHandler.EXTHealingType.All));
                        allyHealingPowerHealing1S.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, friendly, HealingStatsExtensionHandler.EXTHealingType.HealingPower));
                        allyConversionHealingHealing1S.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, friendly, HealingStatsExtensionHandler.EXTHealingType.ConversionBased));
                        allyHybridHealing1S.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, friendly, HealingStatsExtensionHandler.EXTHealingType.Hybrid));
                    }
                    allyHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(a.EXTHealing.GetOutgoingHealEvents(friendly, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc));
                }
            }
            foreach (PhaseData phase in phases)
            {
                outgoingHealing.Add(EXTJsonHealingStatsBuilderCommons.BuildOutgoingHealingStatistics(a.EXTHealing.GetOutgoingHealStats(null, log, phase.Start, phase.End)));
                incomingHealing.Add(EXTJsonHealingStatsBuilderCommons.BuildIncomingHealingStatistics(a.EXTHealing.GetIncomingHealStats(null, log, phase.Start, phase.End)));
                if (settings.RawFormatTimelineArrays)
                {
                    healing1S.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.All));
                    healingPowerHealing1S.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.HealingPower));
                    conversionHealingHealing1S.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.ConversionBased));
                    hybridHealing1S.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.Hybrid));
                }
                totalHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(a.EXTHealing.GetOutgoingHealEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc));
                totalIncomingHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(a.EXTHealing.GetIncomingHealEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc));
            }
            if (!settings.RawFormatTimelineArrays)
            {
                res.AlliedHealing1S = null;
                res.AlliedHealingPowerHealing1S = null;
                res.AlliedConversionHealingHealing1S = null;
                res.AlliedHybridHealing1S = null;
                res.Healing1S = null;
                res.HealingPowerHealing1S = null;
                res.ConversionHealingHealing1S = null;
                res.HybridHealing1S = null;
            }
            return res;
        }
    }
}
