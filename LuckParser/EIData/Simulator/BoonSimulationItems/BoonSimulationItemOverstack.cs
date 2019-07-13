using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using System.Collections.Generic;

namespace LuckParser.EIData
{
    public class BoonSimulationItemOverstack : AbstractBoonSimulationItemWasted
    {

        public BoonSimulationItemOverstack(AgentItem src, long overstack, long time) : base(src, overstack, time)
        {
        }

        public override void SetBoonDistributionItem(BoonDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AgentItem, BoonDistributionItem> distrib = GetDistrib(distribs, boonid);
            AgentItem agent = Src;
            if (distrib.TryGetValue(agent, out var toModify))
            {
                toModify.Overstack += GetValue(start, end);
                distrib[agent] = toModify;
            }
            else
            {
                distrib.Add(agent, new BoonDistributionItem(
                    0,
                    GetValue(start, end), 0, 0, 0, 0));
            }
        }
    }
}
