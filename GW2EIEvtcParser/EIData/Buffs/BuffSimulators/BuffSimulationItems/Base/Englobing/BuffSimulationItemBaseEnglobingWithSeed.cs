using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseEnglobingWithSeed : BuffSimulationItemBaseEnglobing
{
    internal readonly AgentItem _seedSrc;

    protected internal BuffSimulationItemBaseEnglobingWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        _seedSrc = buffStackItem.SeedSrc;
    }
    internal override long GetKey()
    {
        return base.GetKey() + (_seedSrc.InstID + 1) * 65536;
    }
    internal override void SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end, long cDur)
    {
        base.SetBaseBuffDistributionItem(distribution, start, end, cDur);
        AddExtended(distribution, cDur, _seedSrc);
        if (Src.IsUnknown)
        {
            AddUnknown(distribution, cDur, _seedSrc);
        }
    }
}
