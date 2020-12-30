using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal class OverrideLogic : StackingLogic
    {
        // Prefer using Add, to insert to the list in a sorted manner
        protected override void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks)
        {
            stacks.Sort((x, y) => x.TotalDuration.CompareTo(y.TotalDuration));
        }

        public override bool StackEffect(ParsedEvtcLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes)
        {
            if (!stacks.Any())
            {
                return false;
            }
            BuffStackItem stack = stacks[0];
            if (stack.TotalDuration <= stackItem.TotalDuration + ParserHelper.BuffSimulatorDelayConstant)
            {
                wastes.Add(new BuffSimulationItemWasted(stack.Src, stack.Duration, stack.Start));
                if (stack.Extensions.Count > 0)
                {
                    foreach ((AgentItem src, long value) in stack.Extensions)
                    {
                        wastes.Add(new BuffSimulationItemWasted(src, value, stack.Start));
                    }
                }
                stacks.RemoveAt(0);
                Add(log, stacks, stackItem);
                return true;
            }
            else
            {
                return false;
            }
        }

        // https://www.c-sharpcorner.com/blogs/binary-search-implementation-using-c-sharp1
        private static int BinarySearchRecursive(List<BuffStackItem> stacks, long key, int min, int max)
        {
            if (stacks[min].TotalDuration > key)
            {
                return min;
            }
            if (stacks[max].TotalDuration < key)
            {
                return max + 1;
            }
            if (min > max)
            {
               return min;
            }
            else
            {
                int mid = (min + max) / 2;
                if (key == stacks[mid].TotalDuration)
                {
                    return ++mid;
                }
                else if (key < stacks[mid].TotalDuration)
                {
                    return BinarySearchRecursive(stacks, key, min, mid - 1);
                }
                else
                {
                    return BinarySearchRecursive(stacks, key, mid + 1, max);
                }
            }
        }

        public override void Add(ParsedEvtcLog log, List<BuffStackItem> stacks, BuffStackItem stackItem)
        {
            if (!stacks.Any())
            {
                stacks.Add(stackItem);
                return;
            }
            int insertIndex = BinarySearchRecursive(stacks, stackItem.TotalDuration, 0, stacks.Count - 1);
            if (insertIndex > stacks.Count - 1)
            {
                stacks.Add(stackItem);
            }
            else
            {
                stacks.Insert(insertIndex, stackItem);
            }
        }
    }
}
