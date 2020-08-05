using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    public class ChartDataDto
    {
        public List<PhaseChartDataDto> Phases { get; internal set; } = new List<PhaseChartDataDto>();
        public List<MechanicChartDataDto> Mechanics { get; internal set; } = new List<MechanicChartDataDto>();

        internal static List<object[]> BuildHealthGraphStates(ParsedEvtcLog log, AbstractSingleActor actor, PhaseData phase, bool nullable)
        {
            List<Segment> segments = actor.GetHealthUpdates(log);
            if (!segments.Any())
            {
                return nullable ? null : new List<object[]>()
                {
                    new object[] { Math.Round(phase.Start/1000.0, 3), 100.0},
                    new object[] { Math.Round(phase.End/1000.0, 3), 100.0},
                };
            }
            var res = new List<object[]>();
            var subSegments = segments.Where(x => x.End >= phase.Start && x.Start <= phase.End
            ).ToList();
            return Segment.ToObjectList(subSegments, phase.Start, phase.End);
        }
        internal static List<object[]> BuildBreakbarPercentStates(ParsedEvtcLog log, NPC npc, PhaseData phase)
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

        internal static ChartDataDto BuildChartData(ParsedEvtcLog log)
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
                    phaseData.TargetsBreakbarPercentStatesForCR = new List<List<object[]>>();
                    foreach (NPC target in log.FightData.Logic.Targets)
                    {
                        phaseData.TargetsHealthStatesForCR.Add(BuildHealthGraphStates(log, target, phases[0], false));
                        phaseData.TargetsBreakbarPercentStatesForCR.Add(BuildBreakbarPercentStates(log, target, phases[0]));
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
