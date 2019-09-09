using System.Collections.Generic;

namespace LuckParser.Builders.HtmlModels
{
    public class PhaseChartDataDto
    {
        public List<PlayerChartDataDto> Players { get; set; } = new List<PlayerChartDataDto>();
        public List<TargetChartDataDto> Targets { get; set; } = new List<TargetChartDataDto>();

        public List<double[]> TargetsHealthForCR { get; set; } = null;
    }
}
