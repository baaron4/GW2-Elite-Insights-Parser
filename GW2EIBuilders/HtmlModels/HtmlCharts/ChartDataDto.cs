using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal class ChartDataDto
{
    public readonly List<PhaseChartDataDto> Phases = [];
    public readonly List<MechanicChartDataDto> Mechanics = [];

    private static List<double[]>? BuildGraphStates(IReadOnlyList<GenericSegment<double>> segments, PhaseData phase, bool nullable, double defaultState)
    {
        if (!segments.Any())
        {
            return nullable ? null :
            [
                [0.0, defaultState],
                [Math.Round(phase.DurationInMS/1000.0, 3), defaultState],
            ];
        }
        var subSegments = segments.Where(x => x.End >= phase.Start && x.Start <= phase.End).ToList();
        return subSegments.ToObjectList(phase.Start, phase.End);
    }

    public static List<double[]>? BuildHealthStates(ParsedEvtcLog log, SingleActor actor, PhaseData phase, bool nullable)
    {
        return BuildGraphStates(actor.GetHealthUpdates(log), phase, nullable, 100.0);
    }

    public static List<double[]>? BuildBarrierStates(ParsedEvtcLog log, SingleActor actor, PhaseData phase)
    {
        var barriers = new List<GenericSegment<double>>(actor.GetBarrierUpdates(log));
        if (!barriers.Any(x => x.Value > 0))
        {
            barriers.Clear();
        }
        return BuildGraphStates(barriers, phase, true, 0.0);
    }

    public static List<double[]>? BuildBreakbarPercentStates(ParsedEvtcLog log, SingleActor npc, PhaseData phase)
    {
        return BuildGraphStates(npc.GetBreakbarPercentUpdates(log), phase, true, 100.0);
    }

    public ChartDataDto(ParsedEvtcLog log)
    {
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        var phaseChartData = new List<PhaseChartDataDto>(phases.Count);
        for (int i = 0; i < phases.Count; i++)
        {
            phaseChartData.Add(new PhaseChartDataDto(log, phases[i], i == 0));
        }
        Phases = phaseChartData;
        Mechanics = MechanicChartDataDto.BuildMechanicsChartData(log);
    }
}
