using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithSeed : BuffSimulationItemBase
{
    internal readonly AgentItem _seedSrc;

    protected internal BuffSimulationItemBaseWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        _seedSrc = buffStackItem.SeedSrc;
    }
    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long cDur = base.SetBuffDistributionItem(distribs, start, end, buffID);
        if (cDur > 0)
        {
            Dictionary<AgentItem, BuffDistributionItem> distribution = distribs.GetDistrib(buffID);
            AddExtended(distribution, cDur, _seedSrc);
            if (Src.IsUnknown)
            {
                AddUnknown(distribution, cDur, _seedSrc);
            }
            foreach (var subSeedSrc in _seedSrc.EnglobedAgentItems)
            {
                long subcDur = GetClampedDuration(Math.Max(start, subSeedSrc.FirstAware), Math.Min(end, subSeedSrc.LastAware));
                if (subcDur > 0)
                {
                    AddExtended(distribution, subcDur, subSeedSrc);
                    if (Src.IsUnknown)
                    {
                        AddUnknown(distribution, subcDur, subSeedSrc);
                    }
                }
            }

        }
        return cDur;
    }
}
