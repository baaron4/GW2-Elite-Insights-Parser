using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal class PhaseChartDataDto
{
    public List<PlayerChartDataDto>? Players { get; set; }
    public List<TargetChartDataDto>? Targets { get; set; }

    public List<List<double[]>?>? TargetsHealthStatesForCR { get; set; } = null;
    public List<List<double[]>?>? TargetsBreakbarPercentStatesForCR { get; set; } = null;
    public List<List<double[]>?>? TargetsBarrierStatesForCR { get; set; } = null;

    public PhaseChartDataDto(ParsedEvtcLog log, PhaseData phase, bool addCRData)
    {
        Players = PlayerChartDataDto.BuildPlayersGraphData(log, phase);
        Targets = new(phase.Targets.Count);
        foreach (SingleActor target in phase.Targets.Keys)
        {
            Targets.Add(new TargetChartDataDto(log, phase, target));
        }
        if (addCRData)
        {
            TargetsHealthStatesForCR = new(log.FightData.Logic.Targets.Count);
            TargetsBreakbarPercentStatesForCR = new(log.FightData.Logic.Targets.Count);
            TargetsBarrierStatesForCR = new(log.FightData.Logic.Targets.Count);
            foreach (SingleActor target in log.FightData.Logic.Targets)
            {
                TargetsHealthStatesForCR.Add(ChartDataDto.BuildHealthStates(log, target, phase, false));
                TargetsBreakbarPercentStatesForCR.Add(ChartDataDto.BuildBreakbarPercentStates(log, target, phase));
                TargetsBarrierStatesForCR.Add(ChartDataDto.BuildBarrierStates(log, target, phase));
            }
        }
    }
}
