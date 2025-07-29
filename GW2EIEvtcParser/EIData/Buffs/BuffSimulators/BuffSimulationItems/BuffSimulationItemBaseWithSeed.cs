using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithSeed : BuffSimulationItemBase
{
    internal readonly AgentItem _seedSrc;

    protected internal BuffSimulationItemBaseWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
        _seedSrc = buffStackItem.SeedSrc;
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
                    0,
                    0,
                    0,
                    0,
                    cDur
                ));
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
                        0,
                        0,
                        cDur,
                        0,
                        0
                    ));
                }
            }
            foreach (var subSeedSrc in _seedSrc.EnglobedAgentItems)
            {
                long subcDur = GetClampedDuration(Math.Max(start, subSeedSrc.FirstAware), Math.Min(end, subSeedSrc.LastAware));
                if (subcDur > 0)
                {
                    if (distribution.TryGetValue(subSeedSrc, out toModify))
                    {
                        toModify.IncrementExtended(subcDur);
                    }
                    else
                    {
                        distribution.Add(subSeedSrc, new BuffDistributionItem(
                            0,
                            0,
                            0,
                            0,
                            0,
                            subcDur
                        ));
                    }
                    if (_src.IsUnknown)
                    {
                        if (distribution.TryGetValue(subSeedSrc, out toModify))
                        {
                            toModify.IncrementUnknownExtension(subcDur);
                        }
                        else
                        {
                            distribution.Add(subSeedSrc, new BuffDistributionItem(
                                0,
                                0,
                                0,
                                subcDur,
                                0,
                                0
                            ));
                        }
                    }
                }
            }
        }
        return cDur;
    }
}
