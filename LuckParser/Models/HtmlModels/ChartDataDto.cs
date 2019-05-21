using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{   
    public class ChartDataDto
    {      
        public List<PhaseChartDataDto> Phases = new List<PhaseChartDataDto>();      
        public List<MechanicChartDataDto> Mechanics = new List<MechanicChartDataDto>();
    }
}
