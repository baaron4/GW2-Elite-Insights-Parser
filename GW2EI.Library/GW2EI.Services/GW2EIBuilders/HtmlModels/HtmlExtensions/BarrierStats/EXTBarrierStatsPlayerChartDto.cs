using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.EXTBarrier;

internal class EXTBarrierStatsPlayerChartDto
{
    public readonly PlayerDamageChartDto<int> Barrier;

    private EXTBarrierStatsPlayerChartDto(ParsedEvtcLog log, PhaseData phase, SingleActor p)
    {
        Barrier = new PlayerDamageChartDto<int>()
        {
            Total = p.EXTBarrier.Get1SBarrierList(log, phase.Start, phase.End, null),
            Taken = p.EXTBarrier.Get1SBarrierReceivedList(log, phase.Start, phase.End, null),
            Targets = new(log.Friendlies.Count)
        };
        foreach (SingleActor target in log.Friendlies)
        {
            Barrier.Targets.Add(p.EXTBarrier.Get1SBarrierList(log, phase.Start, phase.End, target));
        }
    }

    public static List<EXTBarrierStatsPlayerChartDto> BuildPlayersBarrierGraphData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<EXTBarrierStatsPlayerChartDto>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new EXTBarrierStatsPlayerChartDto(log, phase, actor));
        }
        return list;
    }
}
