using System.Collections.Generic;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;

namespace LuckParser.EIData
{
    public abstract class AbstractBoonSimulationItem
    {

        protected static Dictionary<AgentItem, BoonDistributionItem> GetDistrib(BoonDistributionDictionary distribs, long boonid)
        {
            if (!distribs.TryGetValue(boonid, out Dictionary<AgentItem, BoonDistributionItem> distrib))
            {
                distrib = new Dictionary<AgentItem, BoonDistributionItem>();
                distribs.Add(boonid, distrib);
            }
            return distrib;
        }

        public abstract void SetBoonDistributionItem(BoonDistributionDictionary distribs, long start, long end, long boonid, ParsedLog log);
    }
}
