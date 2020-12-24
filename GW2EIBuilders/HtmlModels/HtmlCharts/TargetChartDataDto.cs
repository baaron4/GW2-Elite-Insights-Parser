using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class TargetChartDataDto
    {
        public IReadOnlyList<int> Total { get; set; }
        public List<object[]> HealthStates { get; set; }
        public List<object[]> BreakbarPercentStates { get; set; }

        public static TargetChartDataDto BuildTargetGraphData(ParsedEvtcLog log, int phaseIndex, NPC target)
        {
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            return new TargetChartDataDto
            {
                Total = target.Get1SDamageList(log, phaseIndex, phase, null),
                HealthStates = ChartDataDto.BuildHealthGraphStates(log, target, log.FightData.GetPhases(log)[phaseIndex], false),
                BreakbarPercentStates = ChartDataDto.BuildBreakbarPercentStates(log, target, log.FightData.GetPhases(log)[phaseIndex]),
            };
        }
    }
}
