using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulationItemOverstack : AbstractBuffSimulationItemWasted
    {

        public BuffSimulationItemOverstack(AgentItem src, long overstack, long time) : base(src, overstack, time)
        {
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID, ParsedEvtcLog log)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
            AgentItem agent = Src;
            long value = GetValue(start, end);
            if (value == 0)
            {
                return;
            }
            if (distrib.TryGetValue(agent, out BuffDistributionItem toModify))
            {
                toModify.IncrementOverstack(value);
            }
            else
            {
                distrib.Add(agent, new BuffDistributionItem(
                    0,
                    value, 0, 0, 0, 0));
            }
        }
    }
}
