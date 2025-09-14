using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithExtensionWithSeedEnglobing : BuffSimulationItemBaseWithSeedEnglobing
{

    protected internal BuffSimulationItemBaseWithExtensionWithSeedEnglobing(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    internal override void SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end, long cDur)
    {
        base.SetBaseBuffDistributionItem(distribution, start, end, cDur);
        AddExtension(distribution, cDur, Src);
    }
}
