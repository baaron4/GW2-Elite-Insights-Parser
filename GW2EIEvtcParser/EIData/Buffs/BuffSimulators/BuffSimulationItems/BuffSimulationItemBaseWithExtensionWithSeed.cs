using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithExtensionWithSeed : BuffSimulationItemBaseWithSeed
{

    protected internal BuffSimulationItemBaseWithExtensionWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long cDur = base.SetBuffDistributionItem(distribs, start, end, buffID);
        if (cDur > 0)
        {
            if (_src.EnglobedAgentItems.Count == 0)
            {
                Dictionary<AgentItem, BuffDistributionItem> distribution = distribs.GetDistrib(buffID);
                if (distribution.TryGetValue(_src, out var toModify))
                {
                    toModify.IncrementExtension(cDur);
                }
                else
                {
                    distribution.Add(_src, new BuffDistributionItem(
                        0,
                        0,
                        0,
                        0,
                        cDur,
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
                        Dictionary<AgentItem, BuffDistributionItem> distribution = distribs.GetDistrib(buffID);
                        if (distribution.TryGetValue(subSrc, out var toModify))
                        {
                            toModify.IncrementExtension(subcDur);
                        }
                        else
                        {
                            distribution.Add(subSrc, new BuffDistributionItem(
                                0,
                                0,
                                0,
                                0,
                                subcDur,
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
