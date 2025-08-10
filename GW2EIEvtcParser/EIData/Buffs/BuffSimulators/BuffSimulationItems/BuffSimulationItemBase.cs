using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBase : BuffSimulationItem
{
    public readonly AgentItem Src;
    //internal readonly long _totalDuration;

    protected internal BuffSimulationItemBase(BuffStackItem buffStackItem) : base(buffStackItem.Start, buffStackItem.Start + buffStackItem.Duration)
    {
        //NOTE(Rennorb): We need to copy these because for some ungodly reason buffsStackItems can change after this initializer runs.
        // this only influences buff uptime values, so it can be difficult to spot.
        // There is a regression test for this in Tests/Regression.cs:BuffUptime.
        Src = buffStackItem.Src;
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
        return [Src];
    }

    public override IEnumerable<AgentItem> GetActiveSources()
    {
        return GetSources();
    }

    private static void Add(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem src)
    {
        if (distrib.TryGetValue(src, out var toModify))
        {
            toModify.IncrementValue(value);
        }
        else
        {
            distrib.Add(src, new BuffDistributionItem(
                value,
                0,
                0,
                0,
                0,
                0
            ));
        }
    }

    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long cDur = GetClampedDuration(start, end);
        if (cDur > 0)
        {
            Dictionary<AgentItem, BuffDistributionItem> distribution = distribs.GetDistrib(buffID);
            Add(distribution, cDur, Src);
            foreach (var subSrc in Src.EnglobedAgentItems)
            {
                long subcDur = GetClampedDuration(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
                if (subcDur > 0)
                {
                    Add(distribution, subcDur, subSrc);
                }
            }
        }

        return cDur;
    }
}
