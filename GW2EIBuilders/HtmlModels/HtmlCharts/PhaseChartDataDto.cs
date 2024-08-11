using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts
{
    internal class PhaseChartDataDto
    {
        public List<PlayerChartDataDto> Players { get; set; } = new List<PlayerChartDataDto>();
        public List<TargetChartDataDto> Targets { get; set; } = new List<TargetChartDataDto>();

        public List<List<object[]>> TargetsHealthStatesForCR { get; set; } = null;
        public List<List<object[]>> TargetsBreakbarPercentStatesForCR { get; set; } = null;
        public List<List<object[]>> TargetsBarrierStatesForCR { get; set; } = null;

        public PhaseChartDataDto(ParsedEvtcLog log, PhaseData phase, bool addCRData)
        {
            Players = PlayerChartDataDto.BuildPlayersGraphData(log, phase);
            foreach (AbstractSingleActor target in phase.AllTargets)
            {
                Targets.Add(new TargetChartDataDto(log, phase, target));
            }
            if (addCRData)
            {
                TargetsHealthStatesForCR = new List<List<object[]>>();
                TargetsBreakbarPercentStatesForCR = new List<List<object[]>>();
                TargetsBarrierStatesForCR = new List<List<object[]>>();
                foreach (AbstractSingleActor target in log.FightData.Logic.Targets)
                {
                    TargetsHealthStatesForCR.Add(ChartDataDto.BuildHealthStates(log, target, phase, false));
                    TargetsBreakbarPercentStatesForCR.Add(ChartDataDto.BuildBreakbarPercentStates(log, target, phase));
                    TargetsBarrierStatesForCR.Add(ChartDataDto.BuildBarrierStates(log, target, phase));
                }
            }
        }
    }
}
