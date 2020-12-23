using System.Collections.Generic;

namespace GW2EIBuilders.HtmlModels
{
    internal class PlayerDamageChartDto<T>
    {
        public List<IReadOnlyList<T>> Targets { get; set; }
        public IReadOnlyList<T> Total { get; set; }
    }
}
