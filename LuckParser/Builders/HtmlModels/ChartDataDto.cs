using System.Collections.Generic;

namespace LuckParser.Builders.HtmlModels
{
    public class ChartDataDto
    {
        public List<PhaseChartDataDto> Phases { get; set; } = new List<PhaseChartDataDto>();
        public List<MechanicChartDataDto> Mechanics { get; set; } = new List<MechanicChartDataDto>();
    }
}
