using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class PlayerDamageChartDto
    {
        public List<List<int>> Targets { get; set; }
        public List<int> Total { get; set; }
    }
}
