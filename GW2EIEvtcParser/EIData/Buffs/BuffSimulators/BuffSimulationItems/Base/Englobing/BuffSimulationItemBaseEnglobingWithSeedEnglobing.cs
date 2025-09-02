using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseEnglobingWithSeedEnglobing : BuffSimulationItemBaseEnglobing
{
    public readonly IReadOnlyList<AgentItem> EnglobedSeedSrcs;

    protected internal BuffSimulationItemBaseEnglobingWithSeedEnglobing(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        var seedSrc = buffStackItem.SeedSrc;
        EnglobedSeedSrcs = seedSrc.EnglobedAgentItems.Where(subSrc => !( Start >= subSrc.LastAware || End <= subSrc.FirstAware)).ToList();
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
