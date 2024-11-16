using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal abstract class ActorChartDataDto
{
    public readonly List<object[]>? HealthStates;
    public readonly List<object[]>? BarrierStates;

    public ActorChartDataDto(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor, bool nullableHPStates)
    {
        HealthStates = ChartDataDto.BuildHealthStates(log, actor, phase, nullableHPStates);
        BarrierStates = ChartDataDto.BuildBarrierStates(log, actor, phase);
    }
}
