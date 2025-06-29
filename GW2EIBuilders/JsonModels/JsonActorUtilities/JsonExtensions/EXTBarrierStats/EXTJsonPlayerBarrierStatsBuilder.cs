using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTBarrier;

internal static class EXTJsonPlayerBarrierStatsBuilder
{

    public static EXTJsonPlayerBarrierStats BuildPlayerBarrierStats(SingleActor a, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
    {
        IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
        var outgoingBarrierAllies = new List<List<EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics>>(log.Friendlies.Count);
        var outgoingBarrier = new List<EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics>(phases.Count);
        var incomingBarrier = new List<EXTJsonBarrierStatistics.EXTJsonIncomingBarrierStatistics>(phases.Count);
        var alliedBarrier1S = new List<List<IReadOnlyList<int>>>(log.Friendlies.Count);
        var barrier1S = new List<IReadOnlyList<int>>(phases.Count);
        var barrierReceived1S = new List<IReadOnlyList<int>>(phases.Count);
        var alliedBarrierDist = new List<List<List<EXTJsonBarrierDist>>>(log.Friendlies.Count);
        var totalBarrierDist = new List<List<EXTJsonBarrierDist>>(phases.Count);
        var totalIncomingBarrierDist = new List<List<EXTJsonBarrierDist>>(phases.Count);

        foreach (SingleActor friendly in log.Friendlies)
        {
            var outgoingBarrierAlly = new List<EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics>(phases.Count);
            var allyBarrier1S = new List<IReadOnlyList<int>>(phases.Count);
            var allyBarrierDist = new List<List<EXTJsonBarrierDist>>(phases.Count);
            
            foreach (PhaseData phase in phases)
            {
                outgoingBarrierAlly.Add(EXTJsonBarrierStatsBuilderCommons.BuildOutgoingBarrierStatistics(a.EXTBarrier.GetOutgoingBarrierStats(friendly, log, phase.Start, phase.End)));
                if (settings.RawFormatTimelineArrays)
                {
                    allyBarrier1S.Add(a.EXTBarrier.Get1SBarrierList(log, phase.Start, phase.End, friendly));
                }
                allyBarrierDist.Add(EXTJsonBarrierStatsBuilderCommons.BuildBarrierDistList(a.EXTBarrier.GetOutgoingBarrierEvents(friendly, log, phase.Start, phase.End).GroupBy(x => x.SkillID), log, skillMap, buffMap).ToList());
            }

            outgoingBarrierAllies.Add(outgoingBarrierAlly);
            alliedBarrier1S.Add(allyBarrier1S);
            alliedBarrierDist.Add(allyBarrierDist);
        }

        foreach (PhaseData phase in phases)
        {
            outgoingBarrier.Add(EXTJsonBarrierStatsBuilderCommons.BuildOutgoingBarrierStatistics(a.EXTBarrier.GetOutgoingBarrierStats(null, log, phase.Start, phase.End)));
            incomingBarrier.Add(EXTJsonBarrierStatsBuilderCommons.BuildIncomingBarrierStatistics(a.EXTBarrier.GetIncomingBarrierStats(null, log, phase.Start, phase.End)));
            if (settings.RawFormatTimelineArrays)
            {
                barrier1S.Add(a.EXTBarrier.Get1SBarrierList(log, phase.Start, phase.End, null));
                barrierReceived1S.Add(a.EXTBarrier.Get1SBarrierReceivedList(log, phase.Start, phase.End, null));
            }
            totalBarrierDist.Add(EXTJsonBarrierStatsBuilderCommons.BuildBarrierDistList(a.EXTBarrier.GetOutgoingBarrierEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID), log, skillMap, buffMap).ToList());
            totalIncomingBarrierDist.Add(EXTJsonBarrierStatsBuilderCommons.BuildBarrierDistList(a.EXTBarrier.GetIncomingBarrierEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID), log, skillMap, buffMap).ToList());
        }

        if (!settings.RawFormatTimelineArrays)
        {
            alliedBarrier1S = null;
            barrier1S = null;
        }

        return new()
        {
            OutgoingBarrier = outgoingBarrier,
            OutgoingBarrierAllies = outgoingBarrierAllies,
            IncomingBarrier = incomingBarrier,
            AlliedBarrier1S = alliedBarrier1S,
            Barrier1S = barrier1S,
            BarrierReceived1S = barrierReceived1S,
            AlliedBarrierDist = alliedBarrierDist,
            TotalBarrierDist = totalBarrierDist,
            TotalIncomingBarrierDist = totalIncomingBarrierDist,
        };
    }
}
