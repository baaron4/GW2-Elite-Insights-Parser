using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBoonSimulationItem
    {

        protected Dictionary<AgentItem, BoonDistributionItem> GetDistrib(BoonDistribution distribs, long boonid)
        {
            if (!distribs.TryGetValue(boonid, out var distrib))
            {
                distrib = new Dictionary<AgentItem, BoonDistributionItem>();
                distribs.Add(boonid, distrib);
            }
            return distrib;
        }

        public abstract void SetBoonDistributionItem(BoonDistribution distribs, long start, long end, long boonid, ParsedLog log);
    }
}
