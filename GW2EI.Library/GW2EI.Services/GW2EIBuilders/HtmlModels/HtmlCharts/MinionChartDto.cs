using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal class MinionChartDto
{
    public readonly List<double[]>? HealthStates;
    public readonly List<double[]>? BarrierStates;

    public MinionChartDto(ParsedEvtcLog log, PhaseData phase, NPC minion)
    {
        HealthStates = ChartDataDto.BuildHealthStates(log, minion, phase, true);
        BarrierStates = ChartDataDto.BuildBarrierStates(log, minion, phase);
    }
}
