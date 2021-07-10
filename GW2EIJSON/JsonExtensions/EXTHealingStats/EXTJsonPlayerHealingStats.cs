using System.Collections.Generic;
using System.Linq;


namespace GW2EIJSON
{
    public class EXTJsonPlayerHealingStats
    {
        public IReadOnlyList<IReadOnlyList<EXTJsonHealingStatistics.EXTJsonOutgoingHealingStatistics>> OutgoingHealingAllies { get; set; }
        public IReadOnlyList<EXTJsonHealingStatistics.EXTJsonOutgoingHealingStatistics> OutgoingHealing{ get; set; }
        public IReadOnlyList<EXTJsonHealingStatistics.EXTJsonIncomingHealingStatistics> IncomingHealing { get; set; }


        public IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>> AlliedHealing1S { get; set; }
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>> AlliedHealingPowerHealing1S { get; set; }
        public IReadOnlyList<IReadOnlyList<IReadOnlyList<int>>> AlliedConversionHealingHealing1S { get; set; }


        public IReadOnlyList<IReadOnlyList<int>> Healing1S { get; set; }
        public IReadOnlyList<IReadOnlyList<int>> HealingPowerHealing1S { get; set; }
        public IReadOnlyList<IReadOnlyList<int>> ConversionHealingHealing1S { get; set; }


        public IReadOnlyList<IReadOnlyList<IReadOnlyList<EXTJsonHealingDist>>> AlliedHealingDist { get; set; }
        public IReadOnlyList<IReadOnlyList<EXTJsonHealingDist>> TotalHealingDist { get; set; }
        public IReadOnlyList<IReadOnlyList<EXTJsonHealingDist>> TotalIncomingHealingDist { get; set; }
    }
}
