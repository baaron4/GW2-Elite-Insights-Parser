using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{
    public class BoonChartDataDto
    {
        public long Id;
        public string Color;
        public bool Visible;
        public List<object[]> States;
    }
}
