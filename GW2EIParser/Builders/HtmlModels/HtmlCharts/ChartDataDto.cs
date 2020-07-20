using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class ChartDataDto
    {
        public List<PhaseChartDataDto> Phases { get; set; } = new List<PhaseChartDataDto>();
        public List<MechanicChartDataDto> Mechanics { get; set; } = new List<MechanicChartDataDto>();

        public static List<object[]> BuildHealthGraphStates(ParsedLog log, AbstractSingleActor actor, PhaseData phase, bool nullable)
        {
            List<Segment> segments = actor.GetHealthUpdates(log);
            if (!segments.Any())
            {
                return nullable ? null : new List<object[]>()
                {
                    new object[] {phase.Start, 100.0},
                    new object[] {phase.End, 100.0},
                };
            }
            var res = new List<object[]>();
            var subSegments = segments.Where(x => x.End >= phase.Start && x.Start <= phase.End
            ).ToList();
            return Segment.ToObjectList(subSegments, phase.Start, phase.End);
        }

        public static ChartDataDto BuildChartData(ParsedLog log)
        {
            var chartData = new ChartDataDto();
            var phaseChartData = new List<PhaseChartDataDto>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                var phaseData = new PhaseChartDataDto()
                {
                    Players = PlayerChartDataDto.BuildPlayersGraphData(log, i)
                };
                foreach (NPC target in phases[i].Targets)
                {
                    phaseData.Targets.Add(TargetChartDataDto.BuildTargetGraphData(log, i, target));
                }
                if (i == 0)
                {
                    phaseData.TargetsHealthStatesForCR = new List<List<object[]>>();
                    foreach (NPC target in log.FightData.Logic.Targets)
                    {
                        phaseData.TargetsHealthStatesForCR.Add(BuildHealthGraphStates(log, target, phases[0], false));
                    }
                }

                phaseChartData.Add(phaseData);
            }
            chartData.Phases = phaseChartData;
            chartData.Mechanics = MechanicChartDataDto.BuildMechanicsChartData(log);
            return chartData;
        }
    }
}
