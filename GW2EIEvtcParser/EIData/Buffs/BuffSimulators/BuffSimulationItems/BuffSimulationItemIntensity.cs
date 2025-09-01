using System;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemIntensity : BuffSimulationItemStack
{
    public BuffSimulationItemIntensity(IReadOnlyList<BuffStackItem> stacks) : base(stacks)
    {
        End = Start + Stacks.Max(x => x.Duration);
    }

    public override void OverrideEnd(long end)
    {
        long maxDur = 0;
        foreach (BuffSimulationItemBase stack in Stacks)
        {
            stack.OverrideEnd(end);

            if(stack.Duration > maxDur) { maxDur = stack.Duration; }
        }
        End = Start + maxDur;
    }

    public override int GetActiveStacks()
    {
        return Stacks.Length;
    }

    public override IEnumerable<AgentItem> GetActiveSources()
    {
        return GetSources();
    }

    public override int GetActiveStacks(SingleActor actor)
    {
        return GetStacks(actor);
    }
    /*public override long GetActualDuration()
    {
        return GetActualDurationPerStack().Max();
    }*/

    public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long cDur = GetClampedDuration(start, end);
        if (cDur > 0)
        {
            var distrib = distribs.GetDistrib(buffID);
            foreach (BuffSimulationItemBase item in Stacks)
            {
                item.SetBaseBuffDistributionItem(distrib, start, end, cDur);
            }
        }
    }
}
