using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class AbstractSimulationItem
    {

        protected static Dictionary<AgentItem, BuffDistributionItem> GetDistrib(BuffDistribution distribs, long boonid)
        {
            if (!distribs.TryGetValue(boonid, out Dictionary<AgentItem, BuffDistributionItem> distrib))
            {
                distrib = new Dictionary<AgentItem, BuffDistributionItem>();
                distribs.Add(boonid, distrib);
            }
            return distrib;
        }

        public abstract void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedLog log);
    }
}
