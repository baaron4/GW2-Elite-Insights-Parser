using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemDuration(IReadOnlyList<BuffStackItem> stacks) : BuffSimulationItemStack(stacks)
{
    public override void OverrideEnd(long end)
    {
        Stacks[0].OverrideEnd(end);
        End = end;
    }

    public override int GetActiveStacks()
    {
        return 1;
    }
    public override int GetActiveStacks(SingleActor actor)
    {
        return (GetSources().First().Is(actor.AgentItem)) ? 1 : 0;
    }
    public override IEnumerable<AgentItem> GetActiveSources()
    {
        return GetSources().Take(1);
    }

    /*public override long GetActualDuration()
    {
        return GetActualDurationPerStack().Sum();
    }*/

    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid)
    {
        return Stacks.First().SetBuffDistributionItem(distribs, start, end, boonid);
    }
}
