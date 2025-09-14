using System;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemDuration : BuffSimulationItemStack
{
    protected readonly BuffSimulationItemBase[] Stacks;
    internal BuffSimulationItemDuration(IReadOnlyList<BuffStackItem> stacks) : base(stacks)
    {
        Stacks = GetStacks(stacks);
    }

    public override void OverrideEnd(long end)
    {
        Stacks[0].OverrideEnd(end);
        End = end;
    }
    public override int GetStacks()
    {
        return Stacks.Length;
    }

    public override int GetActiveStacks()
    {
        return 1;
    }

    public override int GetStacks(SingleActor actor)
    {
        //NOTE(Rennorb): This method only gets called for ~5% of the instances created, so we don't create the buffer in the constructor.
        if (StacksPerSource == null)
        {
            if (Stacks.Length > 0)
            {
                StacksPerSource = new(10);
                foreach (var stack in Stacks)
                {
                    StacksPerSource.IncrementValue(stack.Src);
                }
            }
            else
            {
                StacksPerSource = [];
            }
        }

        return StacksPerSource.GetValueOrDefault(actor.AgentItem);
    }
    public override int GetActiveStacks(SingleActor actor)
    {
        return (GetSources().First().Is(actor.AgentItem)) ? 1 : 0;
    }

    public override IEnumerable<AgentItem> GetSources()
    {
        //NOTE(Rennorb): This method only gets called for ~5% of the instances created, so we don't create the buffer in the constructor.
        if (Sources == null)
        {
            var count = Stacks.Length;
            if (count > 0)
            {
                Sources = new AgentItem[count];
                for (int i = 0; i < count; i++)
                {
                    Sources[i] = Stacks[i].Src;
                }
            }
            else
            {
                Sources = []; // this is array.empty, reused object
            }
        }

        return Sources;
    }
    public override IEnumerable<AgentItem> GetActiveSources()
    {
        return GetSources().Take(1);
    }

    public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long cDur = GetClampedDuration(start, end);
        if (cDur > 0)
        {
            var distrib = distribs.GetDistrib(buffID);
            Stacks[0].SetBaseBuffDistributionItem(distrib, start, end, cDur);
        }
    }
}
