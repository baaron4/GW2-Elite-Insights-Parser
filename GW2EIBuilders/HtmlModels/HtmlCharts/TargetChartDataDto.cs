using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class TargetChartDataDto
    {
        public List<int> Total { get; set; }
        public List<object[]> HealthStates { get; set; }
        public List<object[]> BreakbarPercentStates { get; set; }
        public List<object[]> BarrierStates { get; set; }

        public static TargetChartDataDto BuildTargetGraphData(ParsedEvtcLog log, int phaseIndex, NPC target)
        {
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            return new TargetChartDataDto
            {
                Total = target.Get1SDamageList(log, phase.Start, phase.End, null),
                HealthStates = ChartDataDto.BuildHealthStates(log, target, phase, false),
                BreakbarPercentStates = ChartDataDto.BuildBreakbarPercentStates(log, target, phase),
                BarrierStates = ChartDataDto.BuildBarrierStates(log, target, phase),
            };
        }
    }
}
