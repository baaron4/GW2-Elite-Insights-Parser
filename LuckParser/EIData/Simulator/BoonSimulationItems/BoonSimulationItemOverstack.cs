using System.Collections.Generic;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;

namespace LuckParser.EIData
{
    public class BoonSimulationItemOverstack : AbstractBoonSimulationItemWasted
    {

        public BoonSimulationItemOverstack(AgentItem src, long overstack, long time) : base(src, overstack, time)
        {
        }

        public override void SetBoonDistributionItem(BoonDistributionDictionary distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AgentItem, BoonDistributionItem> distrib = GetDistrib(distribs, boonid);
            AgentItem agent = Src;
            if (distrib.TryGetValue(agent, out BoonDistributionItem toModify))
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
