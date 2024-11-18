using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal abstract class ActorChartDataDto
{
    public readonly List<double[]>? HealthStates;
    public readonly List<double[]>? BarrierStates;

    public ActorChartDataDto(ParsedEvtcLog log, PhaseData phase, SingleActor actor, bool nullableHPStates)
    {
        HealthStates = ChartDataDto.BuildHealthStates(log, actor, phase, nullableHPStates);
        BarrierStates = ChartDataDto.BuildBarrierStates(log, actor, phase);
    }
}
