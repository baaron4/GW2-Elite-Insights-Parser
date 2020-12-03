using System.Collections.Generic;

namespace GW2EIBuilders.HtmlModels
{
    internal class PlayerDamageChartDto<T>
    {
        public List<List<T>> Targets { get; set; }
        public List<T> Total { get; set; }
    }
}
