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

        private static List<object[]> BuildBreakbarPercentStates(ParsedLog log, NPC npc, PhaseData phase)
        {
            List<Segment> segments = npc.GetBreakbarPercentUpdates(log);
            if (!segments.Any())
            {
                return null;
            }
            var res = new List<object[]>();
            var subSegments = segments.Where(x => x.End >= phase.Start && x.Start <= phase.End
            ).ToList();
            return Segment.ToObjectList(subSegments, phase.Start, phase.End);
        }

        public static TargetChartDataDto BuildTargetGraphData(ParsedLog log, int phaseIndex, NPC target)
        {
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            return new TargetChartDataDto
            {
                Total = target.Get1SDamageList(log, phaseIndex, phase, null),
                HealthStates = ChartDataDto.BuildHealthGraphStates(log, target, log.FightData.GetPhases(log)[phaseIndex], false),
                BreakbarPercentStates = BuildBreakbarPercentStates(log, target, log.FightData.GetPhases(log)[phaseIndex]),
            };
        }
    }
}
