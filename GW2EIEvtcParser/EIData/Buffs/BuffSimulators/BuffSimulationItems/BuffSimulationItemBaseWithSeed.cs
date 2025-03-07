using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithSeed : BuffSimulationItemBase
{
    internal readonly AgentItem _seedSrc;

    protected internal BuffSimulationItemBaseWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        _seedSrc       = buffStackItem.SeedSrc;
    }
    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long cDur = base.SetBuffDistributionItem(distribs, start, end, buffID);
        if (cDur > 0)
        {
            Dictionary<AgentItem, BuffDistributionItem> distribution = distribs.GetDistrib(buffID);
            if (distribution.TryGetValue(_seedSrc, out var toModify))
            {
                toModify.IncrementExtended(cDur);
            }
            else
            {
                distribution.Add(_seedSrc, new BuffDistributionItem(
                    0,
                    0, 0, 0, 0, cDur));
            }
            if (_src.IsUnknown)
            {
                if (distribution.TryGetValue(_seedSrc, out toModify))
                {
                    toModify.IncrementUnknownExtension(cDur);
                }
                else
                {
                    distribution.Add(_seedSrc, new BuffDistributionItem(
                        0,
                        0, 0, cDur, 0, 0));
                }
            }
        }
        return cDur;
    }
}
