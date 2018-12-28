using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBoonSimulationItem
    {
        public abstract void SetBoonDistributionItem(Dictionary<long, Dictionary<ushort, BoonDistributionItem>> distribs, long start, long end,long boonid);
    }
}
