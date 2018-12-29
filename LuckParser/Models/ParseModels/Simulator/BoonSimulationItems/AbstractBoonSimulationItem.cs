using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBoonSimulationItem
    {

        protected Dictionary<ushort, BoonDistributionItem> GetDistrib(Dictionary<long, Dictionary<ushort, BoonDistributionItem>> distribs, long boonid)
        {
            if (!distribs.TryGetValue(boonid, out var distrib))
            {
                distrib = new Dictionary<ushort, BoonDistributionItem>();
                distribs.Add(boonid, distrib);
            }
            return distrib;
        }

        public abstract void SetBoonDistributionItem(Dictionary<long, Dictionary<ushort, BoonDistributionItem>> distribs, long start, long end, long boonid);
    }
}
