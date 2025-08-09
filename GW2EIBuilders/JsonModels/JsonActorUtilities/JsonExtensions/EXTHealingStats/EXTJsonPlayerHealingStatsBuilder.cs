using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTHealing;

internal static class EXTJsonPlayerHealingStatsBuilder
{

    public static EXTJsonPlayerHealingStats BuildPlayerHealingStats(SingleActor a, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
    {
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        int phasesCountIfRawFormatRequested = settings.RawFormatTimelineArrays ? phases.Count : 0;

        var outgoingHealingAllies = new List<List<EXTJsonHealingStatistics.EXTJsonOutgoingHealingStatistics>>(log.Friendlies.Count);
        var outgoingHealing = new List<EXTJsonHealingStatistics.EXTJsonOutgoingHealingStatistics>(phases.Count);
        var incomingHealing = new List<EXTJsonHealingStatistics.EXTJsonIncomingHealingStatistics>(phases.Count);

        List<List<IReadOnlyList<int>>>? alliedHealing1S;
        List<List<IReadOnlyList<int>>>? alliedHealingPowerHealing1S;
        List<List<IReadOnlyList<int>>>? alliedConversionHealingHealing1S;
        List<List<IReadOnlyList<int>>>? alliedHybridHealing1S;
        List<IReadOnlyList<int>>? healing1S;
        List<IReadOnlyList<int>>? healingPowerHealing1S;
        List<IReadOnlyList<int>>? conversionHealingHealing1S;
        List<IReadOnlyList<int>>? hybridHealing1S;

        if (settings.RawFormatTimelineArrays)
        {
            alliedHealing1S                  = new(log.Friendlies.Count);
            alliedHealingPowerHealing1S      = new(log.Friendlies.Count);
            alliedConversionHealingHealing1S = new(log.Friendlies.Count);
            alliedHybridHealing1S            = new(log.Friendlies.Count);
            healing1S                  = new(phases.Count);
            healingPowerHealing1S      = new(phases.Count);
            conversionHealingHealing1S = new(phases.Count);
            hybridHealing1S            = new(phases.Count);
        }
        else
        {
            alliedHealing1S                  = null;
            alliedHealingPowerHealing1S      = null;
            alliedConversionHealingHealing1S = null;
            alliedHybridHealing1S            = null;
            healing1S                  = null;
            healingPowerHealing1S      = null;
            conversionHealingHealing1S = null;
            hybridHealing1S            = null;
        }

        var healingReceived1S                  = new List<IReadOnlyList<int>>(phasesCountIfRawFormatRequested);
        var healingPowerHealingReceived1S      = new List<IReadOnlyList<int>>(phasesCountIfRawFormatRequested);
        var conversionHealingHealingReceived1S = new List<IReadOnlyList<int>>(phasesCountIfRawFormatRequested);
        var hybridHealingReceived1S            = new List<IReadOnlyList<int>>(phasesCountIfRawFormatRequested);
        var alliedHealingDist                  = new List<List<List<EXTJsonHealingDist>>>(log.Friendlies.Count);
        var totalHealingDist                   = new List<List<EXTJsonHealingDist>>(phases.Count);
        var totalIncomingHealingDist           = new List<List<EXTJsonHealingDist>>(phases.Count);
        
        foreach (SingleActor friendly in log.Friendlies)
        {
            var outgoingHealingAlly = new List<EXTJsonHealingStatistics.EXTJsonOutgoingHealingStatistics>(phases.Count);
            
            var allyHealing1S = new List<IReadOnlyList<int>>(phasesCountIfRawFormatRequested);
            var allyHealingPowerHealing1S = new List<IReadOnlyList<int>>(phasesCountIfRawFormatRequested);
            var allyConversionHealingHealing1S = new List<IReadOnlyList<int>>(phasesCountIfRawFormatRequested);
            var allyHybridHealing1S = new List<IReadOnlyList<int>>(phasesCountIfRawFormatRequested);

            var allyHealingDist = new List<List<EXTJsonHealingDist>>(phases.Count);

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
                allyHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(a.EXTHealing.GetOutgoingHealEvents(friendly, log, phase.Start, phase.End).GroupBy(x => x.SkillID), log, skillMap, buffMap));
            }

            outgoingHealingAllies.Add(outgoingHealingAlly);
            if (settings.RawFormatTimelineArrays)
            {
                alliedHealing1S!.Add(allyHealing1S);
                alliedHealingPowerHealing1S!.Add(allyHealingPowerHealing1S);
                alliedConversionHealingHealing1S!.Add(allyConversionHealingHealing1S);
                alliedHybridHealing1S!.Add(allyHybridHealing1S);
            }
            alliedHealingDist.Add(allyHealingDist);
        }

        foreach (PhaseData phase in phases)
        {
            outgoingHealing.Add(EXTJsonHealingStatsBuilderCommons.BuildOutgoingHealingStatistics(a.EXTHealing.GetOutgoingHealStats(null, log, phase.Start, phase.End)));
            incomingHealing.Add(EXTJsonHealingStatsBuilderCommons.BuildIncomingHealingStatistics(a.EXTHealing.GetIncomingHealStats(null, log, phase.Start, phase.End)));
            if (settings.RawFormatTimelineArrays)
            {
                healing1S!.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.All));
                healingReceived1S.Add(a.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.All));

                healingPowerHealing1S!.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.HealingPower));
                healingPowerHealingReceived1S.Add(a.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.HealingPower));

                conversionHealingHealing1S!.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.ConversionBased));
                conversionHealingHealingReceived1S.Add(a.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.ConversionBased));

                hybridHealing1S!.Add(a.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.Hybrid));
                hybridHealingReceived1S.Add(a.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, HealingStatsExtensionHandler.EXTHealingType.Hybrid));
            }
            totalHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(a.EXTHealing.GetOutgoingHealEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID), log, skillMap, buffMap));
            totalIncomingHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(a.EXTHealing.GetIncomingHealEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID), log, skillMap, buffMap));
        }

        return new EXTJsonPlayerHealingStats()
        {
            OutgoingHealing = outgoingHealing,
            OutgoingHealingAllies = outgoingHealingAllies,
            IncomingHealing = incomingHealing,
            //
            AlliedHealing1S = alliedHealing1S,
            AlliedConversionHealingHealing1S = alliedConversionHealingHealing1S,
            AlliedHealingPowerHealing1S = alliedHealingPowerHealing1S,
            AlliedHybridHealing1S = alliedHybridHealing1S,
            //
            Healing1S = healing1S,
            HealingPowerHealing1S = healingPowerHealing1S,
            ConversionHealingHealing1S = conversionHealingHealing1S,
            HybridHealing1S = hybridHealing1S,
            //
            HealingReceived1S = healingReceived1S,
            HealingPowerHealingReceived1S = healingPowerHealingReceived1S,
            ConversionHealingHealingReceived1S = conversionHealingHealingReceived1S,
            HybridHealingReceived1S = hybridHealingReceived1S,
            //
            AlliedHealingDist = alliedHealingDist,
            TotalHealingDist = totalHealingDist,
            TotalIncomingHealingDist = totalIncomingHealingDist
        };;
    }
}
