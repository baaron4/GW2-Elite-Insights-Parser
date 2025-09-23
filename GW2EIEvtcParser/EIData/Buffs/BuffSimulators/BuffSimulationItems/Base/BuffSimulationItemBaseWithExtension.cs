using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithExtension : BuffSimulationItemBase
{

    protected internal BuffSimulationItemBaseWithExtension(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    internal override long GetKey()
    {
        return base.GetKey() + ExtensionShift;
    }
    internal override void SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end, long cDur, int weight)
    {
        base.SetBaseBuffDistributionItem(distribution, start, end, cDur, weight);
        AddExtension(distribution, weight * cDur, Src);
    }
}
