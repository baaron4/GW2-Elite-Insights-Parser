using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithExtensionWithSeed : BuffSimulationItemBaseWithSeed
{

    protected internal BuffSimulationItemBaseWithExtensionWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    internal override void SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end, long cDur, int weight)
    {
        base.SetBaseBuffDistributionItem(distribution, start, end, cDur, weight);
        AddExtension(distribution, weight * cDur, Src);
    }
}
