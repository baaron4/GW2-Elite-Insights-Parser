using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class TargetChartDataDto : ActorChartDataDto
    {
        public IReadOnlyList<int> Total { get; }
        public List<object[]> BreakbarPercentStates { get; }

        public TargetChartDataDto(ParsedEvtcLog log, PhaseData phase, NPC target) : base(log, phase, target, false)
        {
            Total = target.Get1SDamageList(log, phase.Start, phase.End, null);
            BreakbarPercentStates = ChartDataDto.BuildBreakbarPercentStates(log, target, phase);
        }
    }
}
