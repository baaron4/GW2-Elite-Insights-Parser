using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseEnglobingWithExtensionWithSeed : BuffSimulationItemBaseEnglobingWithSeed
{

    protected internal BuffSimulationItemBaseEnglobingWithExtensionWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    internal override long SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end)
    {
        long cDur = base.SetBaseBuffDistributionItem(distribution, start, end);
        if (cDur > 0)
        {
            AddExtension(distribution, cDur, Src);
            foreach (var subSrc in Src.EnglobedAgentItems)
            {
                long subcDur = GetClampedDuration(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
                if (subcDur > 0)
                {
                    AddExtension(distribution, subcDur, subSrc);
                }
            }

        }
        return cDur;
    }
}
