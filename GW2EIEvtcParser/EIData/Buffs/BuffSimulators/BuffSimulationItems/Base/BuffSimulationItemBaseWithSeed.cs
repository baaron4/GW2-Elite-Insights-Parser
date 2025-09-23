using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithSeed : BuffSimulationItemBase
{
    internal readonly AgentItem _seedSrc;

    protected internal BuffSimulationItemBaseWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        _seedSrc = buffStackItem.SeedSrc;
    }

    internal override long GetKey()
    {
        return base.GetKey() + _seedSrc.InstID * SeedShift;
    }
    internal override void SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end, long cDur, int weight)
    {
        base.SetBaseBuffDistributionItem(distribution, start, end, cDur, weight);
        AddExtended(distribution, weight * cDur, _seedSrc);
        if (Src.IsUnknown)
        {
            AddUnknown(distribution, weight * cDur, _seedSrc);
        }
    }
}
