using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseEnglobing : BuffSimulationItemBase
{

    public readonly IReadOnlyList<AgentItem> EnglobedSrcs;
    protected internal BuffSimulationItemBaseEnglobing(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        EnglobedSrcs = Src.EnglobedAgentItems.Where(subSrc => Math.Min(End, subSrc.LastAware) - Math.Max(Start, subSrc.FirstAware) > 0 ).ToList();
    }

    internal override void SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end, long cDur)
    {
        foreach (var subSrc in EnglobedSrcs)
        {
            long subcDur = GetClampedDuration(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
            if (subcDur > 0)
            {
                AddValue(distribution, subcDur, subSrc);
            }

        }
    }
}
