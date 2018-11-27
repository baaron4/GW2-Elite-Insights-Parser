using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{  
    public class PhaseChartDataDto
    {    
        public List<PlayerChartDataDto> players = new List<PlayerChartDataDto>();      
        public List<TargetChartDataDto> targets = new List<TargetChartDataDto>();
    }
}
