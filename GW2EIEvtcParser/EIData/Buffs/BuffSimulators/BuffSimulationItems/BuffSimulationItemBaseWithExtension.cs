﻿using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithExtension : BuffSimulationItemBase
{

    protected internal BuffSimulationItemBaseWithExtension(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    private static void Add(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem src)
    {
        if (distrib.TryGetValue(src, out var toModify))
        {
            toModify.IncrementExtension(value);
        }
        else
        {
            distrib.Add(src, new BuffDistributionItem(
                0,
                0,
                0,
                0,
                value,
                0
            ));
        }
    }
    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long cDur = base.SetBuffDistributionItem(distribs, start, end, buffID);
        if (cDur > 0)
        {
            Dictionary<AgentItem, BuffDistributionItem> distribution = distribs.GetDistrib(buffID);
            Add(distribution, cDur, _src);
        }
        return cDur;
    }
}
