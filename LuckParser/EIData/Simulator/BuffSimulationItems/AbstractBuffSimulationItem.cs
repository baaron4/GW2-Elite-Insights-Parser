using System.Collections.Generic;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;

namespace LuckParser.EIData
{
    public abstract class AbstractBuffSimulationItem
    {

        protected static Dictionary<AgentItem, BuffDistributionItem> GetDistrib(BuffDistributionDictionary distribs, long boonid)
        {
            if (!distribs.TryGetValue(boonid, out Dictionary<AgentItem, BuffDistributionItem> distrib))
            {
                distrib = new Dictionary<AgentItem, BuffDistributionItem>();
                distribs.Add(boonid, distrib);
            }
            return distrib;
        }

        public abstract void SetBoonDistributionItem(BuffDistributionDictionary distribs, long start, long end, long boonid, ParsedLog log);
    }
}
