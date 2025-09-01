using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithSeedEnglobing : BuffSimulationItemBase
{
    public readonly IReadOnlyList<AgentItem> EnglobedSeedSrcs;

    protected internal BuffSimulationItemBaseWithSeedEnglobing(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        var seedSrc = buffStackItem.SeedSrc;
        EnglobedSeedSrcs = seedSrc.EnglobedAgentItems.Where(subSrc => Math.Min(End, subSrc.LastAware) - Math.Max(Start, subSrc.FirstAware) > 0).ToList();
    }
    internal override void SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end, long cDur)
    {
        base.SetBaseBuffDistributionItem(distribution, start, end, cDur);
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
}
