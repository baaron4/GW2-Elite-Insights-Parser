using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBase : BuffSimulationItem
{
    internal readonly AgentItem _src;
    //internal readonly long _totalDuration;

    protected internal BuffSimulationItemBase(BuffStackItem buffStackItem) : base(buffStackItem.Start, buffStackItem.Start + buffStackItem.Duration)
    {
        //NOTE(Rennorb): We need to copy these because for some ungodly reason buffsStackItems can change after this initializer runs.
        // this only influences buff uptime values, so it can be difficult to spot.
        // There is a regression test for this in Tests/Regression.cs:BuffUptime.
        _src           = buffStackItem.Src;
        //_totalDuration = buffStackItem.TotalDuration;
    }

    public override void OverrideEnd(long end)
    {
        End = end;
    }

    public override int GetActiveStacks()
    {
        return GetStacks();
    }

    public override int GetStacks()
    {
        return 1;
    }

    public override int GetActiveStacks(SingleActor actor)
    {
        return GetStacks(actor);
    }

    public override int GetStacks(SingleActor actor)
    {
        return GetActiveSources().Any(x => x.Is(actor.AgentItem)) ? 1 : 0;
    }

    /*public override IEnumerable<long> GetActualDurationPerStack()
    {
        return [ GetActualDuration() ];
    }

    public override long GetActualDuration()
    {
        return _totalDuration;
    }*/

    public override IEnumerable<AgentItem> GetSources()
    {
        return [ _src ];
    }

    public override IEnumerable<AgentItem> GetActiveSources()
    {
        return GetSources();
    }

    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long cDur = GetClampedDuration(start, end);
        if (cDur > 0)
        {
            if (_src.EnglobedAgentItems.Count == 0)
            {
                Dictionary<AgentItem, BuffDistributionItem> distribution = distribs.GetDistrib(buffID);
                if (distribution.TryGetValue(_src, out var toModify))
                {
                    toModify.IncrementValue(cDur);
                }
                else
                {
                    distribution.Add(_src, new BuffDistributionItem(
                        cDur,
                        0, 
                        0, 
                        0, 
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
                        Dictionary<AgentItem, BuffDistributionItem> distribution = distribs.GetDistrib(buffID);
                        if (distribution.TryGetValue(subSrc, out var toModify))
                        {
                            toModify.IncrementValue(subcDur);
                        }
                        else
                        {
                            distribution.Add(subSrc, new BuffDistributionItem(
                                subcDur,
                                0, 
                                0, 
                                0, 
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
