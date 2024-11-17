using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithSeed : BuffSimulationItemBase
{
    internal readonly AgentItem _seedSrc;

    protected internal BuffSimulationItemBaseWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        _seedSrc       = buffStackItem.SeedSrc;
    }
    public override bool SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        if (base.SetBuffDistributionItem(distribs, start, end, buffID))
        {
            long cDur = GetClampedDuration(start, end);
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
            if (_src == ParserHelper._unknownAgent)
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
            return true;
        }
        return false;
    }
}
