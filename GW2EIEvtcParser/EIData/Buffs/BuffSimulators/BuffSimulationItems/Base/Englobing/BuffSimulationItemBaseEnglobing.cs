using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseEnglobing : BuffSimulationItemBase
{

    public readonly IReadOnlyList<AgentItem> EnglobedSrcs;
    protected internal BuffSimulationItemBaseEnglobing(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        EnglobedSrcs = Src.EnglobedAgentItems;
    }

    internal override long SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end)
    {
        long cDur = GetClampedDuration(start, end);
        if (cDur > 0)
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
        return cDur;
    }
}
