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
            if (_seedSrc.EnglobedAgentItems.Count == 0)
            {
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
            } 
            else
            {
                foreach (var subSrc in _seedSrc.EnglobedAgentItems)
                {
                    long subcDur = GetClampedDuration(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
                    if (subcDur > 0)
                    {
                        if (distribution.TryGetValue(subSrc, out var toModify))
                        {
                            toModify.IncrementExtended(subcDur);
                        }
                        else
                        {
                            distribution.Add(subSrc, new BuffDistributionItem(
                                0,
                                0,
                                0,
                                0,
                                0,
                                subcDur
                            ));
                        }
                    }
                }
            }
            if (_src.IsUnknown)
            {
                if (_src.EnglobedAgentItems.Count == 0)
                {
                    if (distribution.TryGetValue(_seedSrc, out var toModify))
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
                else
                {
                    foreach (var subSrc in _src.EnglobedAgentItems)
                    {
                        long subcDur = GetClampedDuration(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
                        if (subcDur > 0)
                        {
                            if (distribution.TryGetValue(subSrc, out var toModify))
                            {
                                toModify.IncrementUnknownExtension(subcDur);
                            }
                            else
                            {
                                distribution.Add(subSrc, new BuffDistributionItem(
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
        }
        return cDur;
    }
}
