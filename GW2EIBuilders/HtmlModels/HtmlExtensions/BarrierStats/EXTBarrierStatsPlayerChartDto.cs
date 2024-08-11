using System.Collections.Generic;
using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.EXTBarrier
{
    internal class EXTBarrierStatsPlayerChartDto
    {
        public PlayerDamageChartDto<int> Barrier { get; }

        private EXTBarrierStatsPlayerChartDto(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor p)
        {
            Barrier = new PlayerDamageChartDto<int>()
            {
                Total = p.EXTBarrier.Get1SBarrierList(log, phase.Start, phase.End, null),
                Taken = p.EXTBarrier.Get1SBarrierReceivedList(log, phase.Start, phase.End, null),
                Targets = new List<IReadOnlyList<int>>()
            };
            foreach (AbstractSingleActor target in log.Friendlies)
            {
                Barrier.Targets.Add(p.EXTBarrier.Get1SBarrierList(log, phase.Start, phase.End, target));
            }
        }

        public static List<EXTBarrierStatsPlayerChartDto> BuildPlayersBarrierGraphData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<EXTBarrierStatsPlayerChartDto>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                list.Add(new EXTBarrierStatsPlayerChartDto(log, phase, actor));
            }
            return list;
        }
    }
}
