using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithSeedEnglobing : BuffSimulationItemBase
{
    internal readonly AgentItem _seedSrc;
    public readonly IReadOnlyList<AgentItem> EnglobedSeedSrcs;

    protected internal BuffSimulationItemBaseWithSeedEnglobing(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        _seedSrc = buffStackItem.SeedSrc;
        EnglobedSeedSrcs = _seedSrc.EnglobedAgentItems;
    }
    internal override long SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end)
    {
        long cDur = base.SetBaseBuffDistributionItem(distribution, start, end);
        if (cDur > 0)
        {
            foreach (var subSeedSrc in EnglobedSeedSrcs)
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
