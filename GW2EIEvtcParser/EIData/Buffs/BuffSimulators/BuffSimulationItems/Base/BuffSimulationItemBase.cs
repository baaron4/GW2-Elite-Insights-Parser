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

    internal virtual long GetKey()
    {
        return Src.InstID;
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

    public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid)
    {
        throw new InvalidOperationException("Base items can't use that SetBuffDistributionItem");
    }

    internal virtual void SetBaseBuffDistributionItem(Dictionary<AgentItem, BuffDistributionItem> distribution, long start, long end, long cDur, int weight)
    {
        AddValue(distribution, weight * cDur, Src);
    }
}
