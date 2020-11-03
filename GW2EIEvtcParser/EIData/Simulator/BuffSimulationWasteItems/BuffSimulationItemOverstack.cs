﻿using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulationItemOverstack : AbstractBuffSimulationItemWasted
    {

        public BuffSimulationItemOverstack(AgentItem src, long overstack, long time) : base(src, overstack, time)
        {
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedEvtcLog log)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = GetDistrib(distribs, boonid);
            AgentItem agent = Src; 
            var value = GetValue(start, end);
            if (value == 0)
            {
                return;
            }
            if (distrib.TryGetValue(agent, out BuffDistributionItem toModify))
            {
                toModify.Overstack += value;
                distrib[agent] = toModify;
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
