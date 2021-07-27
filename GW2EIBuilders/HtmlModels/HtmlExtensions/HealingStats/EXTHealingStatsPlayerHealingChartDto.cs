using System.Collections.Generic;

namespace GW2EIBuilders.HtmlModels
{
    internal class EXTHealingStatsPlayerHealingChartDto<T>
    {
        public List<IReadOnlyList<T>> Friendlies { get; set; }
        public IReadOnlyList<T> Total { get; set; }
    }
}
