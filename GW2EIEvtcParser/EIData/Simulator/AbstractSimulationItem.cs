using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class AbstractSimulationItem
    {

        protected static Dictionary<AgentItem, BuffDistributionItem> GetDistrib(BuffDistribution distribs, long boonid)
        {
            if (!distribs.Distributions.TryGetValue(boonid, out Dictionary<AgentItem, BuffDistributionItem> distrib))
            {
                distrib = new Dictionary<AgentItem, BuffDistributionItem>();
                distribs.Distributions.Add(boonid, distrib);
            }
            return distrib;
        }

        public abstract void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedEvtcLog log);
    }
}
