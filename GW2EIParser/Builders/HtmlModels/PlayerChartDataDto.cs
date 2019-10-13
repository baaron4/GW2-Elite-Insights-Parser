using System.Collections.Generic;

namespace GW2EIParser.Builders.HtmlModels
{
    public class PlayerChartDataDto
    {
        public List<List<int>> Targets { get; set; }
        public List<int> Total { get; set; }
    }
}
