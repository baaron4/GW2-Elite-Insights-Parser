using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBoonSimulationItem
    {

        protected Dictionary<AbstractActor, BoonDistributionItem> GetDistrib(BoonDistribution distribs, long boonid)
        {
            if (!distribs.TryGetValue(boonid, out var distrib))
            {
                distrib = new Dictionary<AbstractActor, BoonDistributionItem>();
                distribs.Add(boonid, distrib);
            }
            return distrib;
        }

        public abstract void SetBoonDistributionItem(BoonDistribution distribs, long start, long end, long boonid, ParsedLog log);
    }
}
