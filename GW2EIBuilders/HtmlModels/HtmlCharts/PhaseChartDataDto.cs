using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal class PhaseChartDataDto
{
    public List<PlayerChartDataDto> Players { get; set; } = [];
    public List<TargetChartDataDto> Targets { get; set; } = [];

    public List<List<double[]>?>? TargetsHealthStatesForCR { get; set; } = null;
    public List<List<double[]>?>? TargetsBreakbarPercentStatesForCR { get; set; } = null;
    public List<List<double[]>?>? TargetsBarrierStatesForCR { get; set; } = null;

    public PhaseChartDataDto(ParsedEvtcLog log, PhaseData phase, bool addCRData)
    {
        Players = PlayerChartDataDto.BuildPlayersGraphData(log, phase);
        foreach (SingleActor target in phase.AllTargets)
        {
            Targets.Add(new TargetChartDataDto(log, phase, target));
        }
        if (addCRData)
        {
            TargetsHealthStatesForCR = [];
            TargetsBreakbarPercentStatesForCR = [];
            TargetsBarrierStatesForCR = [];
            foreach (SingleActor target in log.FightData.Logic.Targets)
            {
                TargetsHealthStatesForCR.Add(ChartDataDto.BuildHealthStates(log, target, phase, false));
                TargetsBreakbarPercentStatesForCR.Add(ChartDataDto.BuildBreakbarPercentStates(log, target, phase));
                TargetsBarrierStatesForCR.Add(ChartDataDto.BuildBarrierStates(log, target, phase));
            }
        }
    }
}
