using System;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemIntensity : BuffSimulationItemStack
{
    private readonly List<RegroupedStack> RegroupedStacks; 

    private class RegroupedStack
    {
        public required BuffSimulationItemBase Item;
        public int StackCount;
    }
    public BuffSimulationItemIntensity(IReadOnlyList<BuffStackItem> stacks) : base(stacks)
    {
        var simulStacks = GetStacks(stacks);
        End = Start + simulStacks.Max(x => x.Duration);
        var regroupedStacksDict = new Dictionary<long, RegroupedStack>(simulStacks.Length);
        foreach (var simulItem in simulStacks)
        {
            var key = simulItem.GetKey();
            if (regroupedStacksDict.TryGetValue(key, out var list))
            {
                list.StackCount++;
            }
            else
            {
                regroupedStacksDict[key] = new RegroupedStack()
                {
                    Item = simulItem,
                    StackCount = 1
                };
            }
        }
        RegroupedStacks = regroupedStacksDict.Values.ToList();
    }

    public override void OverrideEnd(long end)
    {
        long maxDur = 0;
        foreach (var pair in RegroupedStacks)
        {
            var stack = pair.Item;
            stack.OverrideEnd(end);

            if(stack.Duration > maxDur) { maxDur = stack.Duration; }
        }
        End = Start + maxDur;
    }
    public override int GetStacks()
    {
        return RegroupedStacks.Sum(x => x.StackCount);
    }

    public override int GetActiveStacks()
    {
        return GetStacks();
    }
    public override int GetStacks(SingleActor actor)
    {
        //NOTE(Rennorb): This method only gets called for ~5% of the instances created, so we don't create the buffer in the constructor.
        if (StacksPerSource == null)
        {
            if (RegroupedStacks.Count > 0)
            {
                StacksPerSource = new(10);
                foreach (var stack in RegroupedStacks)
                {
                    StacksPerSource.IncrementValue(stack.Item.Src, stack.StackCount);
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
        return GetStacks(actor);
    }
    public override IEnumerable<AgentItem> GetSources()
    {
        //NOTE(Rennorb): This method only gets called for ~5% of the instances created, so we don't create the buffer in the constructor.
        if (Sources == null)
        {
            var count = GetStacks();
            if (count > 0)
            {
                Sources = new AgentItem[count];
                int offset = 0;
                foreach (var pair in RegroupedStacks)
                {
                    for (int i = 0; i < pair.StackCount; i++)
                    {
                        Sources[offset++] = pair.Item.Src;
                    }
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
        return GetSources();
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
            foreach (var pair in RegroupedStacks)
            {
                pair.Item.SetBaseBuffDistributionItem(distrib, start, end, pair.StackCount * cDur);
            }
        }
    }
}
