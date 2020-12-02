using System.Collections.Generic;

namespace GW2EIBuilders.HtmlModels
{
    internal class PhaseChartDataDto
    {
        public List<PlayerChartDataDto> Players { get; set; } = new List<PlayerChartDataDto>();
        public List<TargetChartDataDto> Targets { get; set; } = new List<TargetChartDataDto>();

        public List<List<object[]>> TargetsHealthStatesForCR { get; set; } = null;
        public List<List<object[]>> TargetsBreakbarPercentStatesForCR { get; set; } = null;
        public List<List<object[]>> TargetsBarrierStatesForCR { get; set; } = null;
    }
}
