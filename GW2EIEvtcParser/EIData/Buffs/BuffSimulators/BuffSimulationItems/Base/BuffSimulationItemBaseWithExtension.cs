using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithExtension : BuffSimulationItemBase
{

    protected internal BuffSimulationItemBaseWithExtension(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    internal override long SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end)
    {
        long cDur = base.SetBaseBuffDistributionItem(distribution, start, end);
        if (cDur > 0)
        {
            AddExtension(distribution!, cDur, Src);
        }
        return cDur;
    }
}
