using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{   
    public class ChartDataDto
    {      
        public List<PhaseChartDataDto> phases = new List<PhaseChartDataDto>();      
        public List<MechanicChartDataDto> mechanics = new List<MechanicChartDataDto>();
    }
}
