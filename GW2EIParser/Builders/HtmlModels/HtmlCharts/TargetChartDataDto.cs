using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class TargetChartDataDto
    {
        public List<int> Total { get; set; }
        public List<object[]> HealthStates { get; set; }
        public List<object[]> BreakbarPercentStates { get; set; }

        public static TargetChartDataDto BuildTargetGraphData(ParsedLog log, int phaseIndex, NPC target)
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
