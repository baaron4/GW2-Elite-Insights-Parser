﻿using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithSeed : BuffSimulationItemBase
{
    internal readonly AgentItem _seedSrc;

    protected internal BuffSimulationItemBaseWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        _seedSrc = buffStackItem.SeedSrc;
    }

    private static void Add(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem seedSrc)
    {
        if (distrib.TryGetValue(seedSrc, out var toModify))
        {
            toModify.IncrementExtended(value);
        }
        else
        {
            distrib.Add(seedSrc, new BuffDistributionItem(
                0,
                0,
                0,
                0,
                0,
                value
            ));
        }
    }
    private static void AddUnknown(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem seedSrc)
    {
        if (distrib.TryGetValue(seedSrc, out var toModify))
        {
            toModify.IncrementUnknownExtension(value);
        }
        else
        {
            distrib.Add(seedSrc, new BuffDistributionItem(
                0,
                0,
                0,
                value,
                0,
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
            Add(distribution, cDur, _seedSrc);
            if (_src.IsUnknown)
            {
                AddUnknown(distribution, cDur, _seedSrc);
            }
        }
        return cDur;
    }
}
