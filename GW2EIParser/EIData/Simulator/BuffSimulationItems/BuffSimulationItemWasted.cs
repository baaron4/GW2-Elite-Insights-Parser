using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemWasted : AbstractBuffSimulationItemWasted
    {

        public BuffSimulationItemWasted(AgentItem src, long waste, long time) : base(src, waste, time)
        {
        }

        public override void SetBoonDistributionItem(BuffDistributionDictionary distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = GetDistrib(distribs, boonid);
            AgentItem agent = Src;
            if (distrib.TryGetValue(agent, out BuffDistributionItem toModify))
            {
                toModify.Waste += GetValue(start, end);
                distrib[agent] = toModify;
            }
            else
            {
                distrib.Add(agent, new BuffDistributionItem(
                    0,
                    0, GetValue(start, end), 0, 0, 0));
            }
        }
    }
}
