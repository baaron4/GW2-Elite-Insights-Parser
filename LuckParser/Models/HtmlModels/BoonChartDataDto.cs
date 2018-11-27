using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{
    public class BoonChartDataDto
    {
        public long id;
        public string color;
        public bool visible;
        public List<double[]> states;
    }
}
