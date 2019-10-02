using System.Collections.Generic;

namespace GW2EIParser.Builders.HtmlModels
{
    public class PhaseChartDataDto
    {
        public List<PlayerChartDataDto> Players { get; set; } = new List<PlayerChartDataDto>();
        public List<TargetChartDataDto> Targets { get; set; } = new List<TargetChartDataDto>();

        public List<double[]> TargetsHealthForCR { get; set; } = null;
    }
}
