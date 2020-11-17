using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class PlayerDamageChartDto<T>
    {
        public List<List<T>> Targets { get; set; }
        public List<T> Total { get; set; }
    }
}
