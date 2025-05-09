﻿using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemWasted : AbstractBuffSimulationItemWasted
{

    public BuffSimulationItemWasted(AgentItem src, long waste, long time) : base(src, waste, time)
    {
    }

    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        Dictionary<AgentItem, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
        long value = GetValue(start, end);
        if (value > 0)
        {
            AgentItem agent = Src;
            if (distrib.TryGetValue(agent, out var toModify))
            {
                toModify.IncrementWaste(value);
            }
            else
            {
                distrib.Add(agent, new BuffDistributionItem(
                    0,
                    0, value, 0, 0, 0));
            }
        }
        return value;
    }
}
