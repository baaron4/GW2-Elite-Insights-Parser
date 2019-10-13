using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemOverstack : AbstractBuffSimulationItemWasted
    {

        public BuffSimulationItemOverstack(AgentItem src, long overstack, long time) : base(src, overstack, time)
        {
        }

        public override void SetBoonDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = GetDistrib(distribs, boonid);
            AgentItem agent = Src;
            if (distrib.TryGetValue(agent, out BuffDistributionItem toModify))
            {
                toModify.Overstack += GetValue(start, end);
                distrib[agent] = toModify;
            }
            else
            {
                distrib.Add(agent, new BuffDistributionItem(
                    0,
                    GetValue(start, end), 0, 0, 0, 0));
            }
        }
    }
}
