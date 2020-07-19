using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class TargetChartDataDto
    {
        public List<int> Total { get; set; }
        public double[] Health { get; set; }

        public static TargetChartDataDto BuildTargetGraphData(ParsedLog log, int phaseIndex, NPC target)
        {
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            return new TargetChartDataDto
            {
                Total = target.Get1SDamageList(log, phaseIndex, phase, null),
                Health = target.Get1SHealthGraph(log)[phaseIndex]
            };
        }
    }
}
