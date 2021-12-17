using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
{
    internal static class EXTJsonMinionsBarrierStatsBuilder
    {

        public static EXTJsonMinionsBarrierStats BuildMinionsBarrierStats(Minions minions, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var totalBarrier = new List<int>();
            var totalAlliedBarrier = new List<List<int>>();
            var alliedBarrierDist = new List<List<List<EXTJsonBarrierDist>>>();
            var totalBarrierDist = new List<List<EXTJsonBarrierDist>>();
            var res = new EXTJsonMinionsBarrierStats()
            {
                TotalBarrier = totalBarrier,
                TotalAlliedBarrier = totalAlliedBarrier,
                AlliedBarrierDist = alliedBarrierDist,
                TotalBarrierDist = totalBarrierDist
            };
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            foreach (AbstractSingleActor friendly in log.Friendlies)
            {
                var totalAllyBarrier = new List<int>();
                totalAlliedBarrier.Add(totalAllyBarrier);
                //
                var allyBarrierDist = new List<List<EXTJsonBarrierDist>>();
                alliedBarrierDist.Add(allyBarrierDist);
                foreach (PhaseData phase in phases)
                {
                    IReadOnlyList<GW2EIEvtcParser.Extensions.EXTAbstractBarrierEvent> list = minions.EXTBarrier.GetOutgoingBarrierEvents(friendly, log, phase.Start, phase.End);
                    totalAllyBarrier.Add(list.Sum(x => x.BarrierGiven));
                    allyBarrierDist.Add(EXTJsonBarrierStatsBuilderCommons.BuildBarrierDistList(list.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc));
                }
            }
            foreach (PhaseData phase in phases)
            {
                IReadOnlyList<GW2EIEvtcParser.Extensions.EXTAbstractBarrierEvent> list = minions.EXTBarrier.GetOutgoingBarrierEvents(null, log, phase.Start, phase.End);
                totalBarrier.Add(list.Sum(x => x.BarrierGiven));
                totalBarrierDist.Add(EXTJsonBarrierStatsBuilderCommons.BuildBarrierDistList(list.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc));
            }
            return res;
        }

    }
}
