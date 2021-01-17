using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal abstract class ActorChartDataDto
    {
        public List<object[]> HealthStates { get; }
        public List<object[]> BarrierStates { get; }

        public ActorChartDataDto(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor, bool nullableHPStates)
        {
            HealthStates = ChartDataDto.BuildHealthStates(log, actor, phase, nullableHPStates);
            BarrierStates = ChartDataDto.BuildBarrierStates(log, actor, phase);
        }
    }
}
