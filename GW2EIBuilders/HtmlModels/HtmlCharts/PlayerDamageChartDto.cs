using System.Collections.Generic;

namespace GW2EIBuilders.HtmlModels.HTMLCharts
{
    internal class PlayerDamageChartDto<T>
    {
        public List<IReadOnlyList<T>> Targets { get; set; }
        public IReadOnlyList<T> Total { get; set; }
        public IReadOnlyList<T> Taken { get; set; }
    }
}
