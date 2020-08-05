using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class TargetChartDataDto
    {
        public List<int> Total { get; internal set; }
        public List<object[]> HealthStates { get; internal set; }
        public List<object[]> BreakbarPercentStates { get; internal set; }

        internal static TargetChartDataDto BuildTargetGraphData(ParsedEvtcLog log, int phaseIndex, NPC target)
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
