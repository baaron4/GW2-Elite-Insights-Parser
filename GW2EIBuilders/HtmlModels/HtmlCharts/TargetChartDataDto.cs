using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class TargetChartDataDto
    {
        public IReadOnlyList<int> Total { get; }
        public List<object[]> HealthStates { get; }
        public List<object[]> BreakbarPercentStates { get; }
        public List<object[]> BarrierStates { get; }

        public TargetChartDataDto(ParsedEvtcLog log, PhaseData phase, NPC target)
        {
            Total = target.Get1SDamageList(log, phase.Start, phase.End, null);
            HealthStates = ChartDataDto.BuildHealthStates(log, target, phase, false);
            BreakbarPercentStates = ChartDataDto.BuildBreakbarPercentStates(log, target, phase);
            BarrierStates = ChartDataDto.BuildBarrierStates(log, target, phase);
        }
    }
}
