using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseEnglobingWithExtension : BuffSimulationItemBaseEnglobing
{

    protected internal BuffSimulationItemBaseEnglobingWithExtension(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    internal override void SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end, long cDur, int weight)
    {
        base.SetBaseBuffDistributionItem(distribution, start, end, cDur, weight);
        foreach (var subSrc in EnglobedSrcs)
        {
            long subcDur = GetClampedDuration(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
            if (subcDur > 0)
            {
                AddExtension(distribution, weight * subcDur, subSrc);
            }

        }
    }
}
