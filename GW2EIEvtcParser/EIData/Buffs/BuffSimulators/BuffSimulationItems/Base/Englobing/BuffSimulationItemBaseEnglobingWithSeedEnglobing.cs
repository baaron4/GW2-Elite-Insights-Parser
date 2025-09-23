using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseEnglobingWithSeedEnglobing : BuffSimulationItemBaseEnglobing
{
    public readonly IReadOnlyList<AgentItem> EnglobedSeedSrcs;
    internal readonly AgentItem _seedSrc;

    protected internal BuffSimulationItemBaseEnglobingWithSeedEnglobing(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        _seedSrc = buffStackItem.SeedSrc;
        EnglobedSeedSrcs = _seedSrc.EnglobedAgentItems.Where(subSrc => !( Start >= subSrc.LastAware || End <= subSrc.FirstAware)).ToList();
    }
    internal override long GetKey()
    {
        return base.GetKey() + _seedSrc.InstID * SeedShift;
    }
    internal override void SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end, long cDur, int weight)
    {
        base.SetBaseBuffDistributionItem(distribution, start, end, cDur, weight);
        foreach (var subSeedSrc in EnglobedSeedSrcs)
        {
            long subcDur = GetClampedDuration(Math.Max(start, subSeedSrc.FirstAware), Math.Min(end, subSeedSrc.LastAware));
            if (subcDur > 0)
            {
                AddExtended(distribution, weight * subcDur, subSeedSrc);
                if (Src.IsUnknown)
                {
                    AddUnknown(distribution, weight * subcDur, subSeedSrc);
                }
            }
        }

    }
}
