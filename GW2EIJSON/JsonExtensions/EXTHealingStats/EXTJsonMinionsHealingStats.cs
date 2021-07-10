using System.Collections.Generic;
using System.Linq;


namespace GW2EIJSON
{
    public class EXTJsonMinionsHealingStats
    {
        public IReadOnlyList<int> TotalHealing { get; set; }
        public IReadOnlyList<IReadOnlyList<int>> TotalAlliedHealing { get; set; }

        public IReadOnlyList<IReadOnlyList<IReadOnlyList<EXTJsonHealingDist>>> AlliedHealingDist { get; set; }
        public IReadOnlyList<IReadOnlyList<EXTJsonHealingDist>> TotalHealingDist { get; set; }

        public EXTJsonMinionsHealingStats()
        {

        }
    }
}
