using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class ChartDataDto
    {
        public List<PhaseChartDataDto> Phases { get; } = new List<PhaseChartDataDto>();
        public List<MechanicChartDataDto> Mechanics { get; } = new List<MechanicChartDataDto>();

        private static List<object[]> BuildGraphStates(IReadOnlyList<Segment> segments, PhaseData phase, bool nullable, double defaultState)
        {
            if (!segments.Any())
            {
                return nullable ? null : new List<object[]>()
                {
                    new object[] { Math.Round(phase.Start/1000.0, 3), defaultState},
                    new object[] { Math.Round(phase.End/1000.0, 3), defaultState},
                };
            }
            var res = new List<object[]>();
            var subSegments = segments.Where(x => x.End >= phase.Start && x.Start <= phase.End
            ).ToList();
            return Segment.ToObjectList(subSegments, phase.Start, phase.End);
        }

        public static List<object[]> BuildHealthStates(ParsedEvtcLog log, AbstractSingleActor actor, PhaseData phase, bool nullable)
        {
            return BuildGraphStates(actor.GetHealthUpdates(log), phase, nullable, 100.0);
        }

        public static List<object[]> BuildBarrierStates(ParsedEvtcLog log, AbstractSingleActor actor, PhaseData phase)
        {
            var barriers = new List<Segment>(actor.GetBarrierUpdates(log));
            if (!barriers.Any(x => x.Value > 0))
            {
                barriers.Clear();
            }
            return BuildGraphStates(barriers, phase, true, 0.0);
        }

        public static List<object[]> BuildBreakbarPercentStates(ParsedEvtcLog log, NPC npc, PhaseData phase)
        {
            return BuildGraphStates(npc.GetBreakbarPercentUpdates(log), phase, true, 100.0);
        }

        public ChartDataDto(ParsedEvtcLog log)
        {
            var phaseChartData = new List<PhaseChartDataDto>();
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                phaseChartData.Add(new PhaseChartDataDto(log, phases[i], i == 0));
            }
            Phases = phaseChartData;
            Mechanics = MechanicChartDataDto.BuildMechanicsChartData(log);
        }
    }
}
