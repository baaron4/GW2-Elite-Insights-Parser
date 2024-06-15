using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTBarrier
{
    internal static class EXTJsonPlayerBarrierStatsBuilder
    {

        public static EXTJsonPlayerBarrierStats BuildPlayerBarrierStats(AbstractSingleActor a, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
        {
            var outgoingBarrierAllies = new List<List<EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics>>();
            var outgoingBarrier = new List<EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics>();
            var incomingBarrier = new List<EXTJsonBarrierStatistics.EXTJsonIncomingBarrierStatistics>();
            var alliedBarrier1S = new List<List<IReadOnlyList<int>>>();
            var barrier1S = new List<IReadOnlyList<int>>();
            var barrierReceived1S = new List<IReadOnlyList<int>>();
            var alliedBarrierDist = new List<List<List<EXTJsonBarrierDist>>>();
            var totalBarrierDist = new List<List<EXTJsonBarrierDist>>();
            var totalIncomingBarrierDist = new List<List<EXTJsonBarrierDist>>();
            var res = new EXTJsonPlayerBarrierStats()
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
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (AbstractSingleActor friendly in log.Friendlies)
            {
                //
                var outgoingBarrierAlly = new List<EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics>();
                outgoingBarrierAllies.Add(outgoingBarrierAlly);
                //
                var allyBarrier1S = new List<IReadOnlyList<int>>();
                alliedBarrier1S.Add(allyBarrier1S);
                //
                var allyBarrierDist = new List<List<EXTJsonBarrierDist>>();
                alliedBarrierDist.Add(allyBarrierDist);
                foreach (PhaseData phase in phases)
                {
                    outgoingBarrierAlly.Add(EXTJsonBarrierStatsBuilderCommons.BuildOutgoingBarrierStatistics(a.EXTBarrier.GetOutgoingBarrierStats(friendly, log, phase.Start, phase.End)));
                    if (settings.RawFormatTimelineArrays)
                    {
                        allyBarrier1S.Add(a.EXTBarrier.Get1SBarrierList(log, phase.Start, phase.End, friendly));
                    }
                    allyBarrierDist.Add(EXTJsonBarrierStatsBuilderCommons.BuildBarrierDistList(a.EXTBarrier.GetOutgoingBarrierEvents(friendly, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillMap, buffMap));
                }
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
                totalBarrierDist.Add(EXTJsonBarrierStatsBuilderCommons.BuildBarrierDistList(a.EXTBarrier.GetOutgoingBarrierEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillMap, buffMap));
                totalIncomingBarrierDist.Add(EXTJsonBarrierStatsBuilderCommons.BuildBarrierDistList(a.EXTBarrier.GetIncomingBarrierEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillMap, buffMap));
            }
            if (!settings.RawFormatTimelineArrays)
            {
                res.AlliedBarrier1S = null;
                res.Barrier1S = null;
            }
            return res;
        }
    }
}
