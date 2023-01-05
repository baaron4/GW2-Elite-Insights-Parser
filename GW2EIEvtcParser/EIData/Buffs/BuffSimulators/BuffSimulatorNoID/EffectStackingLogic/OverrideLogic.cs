using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class OverrideLogic : StackingLogic
    {

        // Prefer using Add, to insert to the list in a sorted manner
        protected override void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks)
        {
            throw new InvalidOperationException("Do not use Sort for OverrideLogic");
        }

        public override bool FindLowestValue(ParsedEvtcLog log, BuffStackItem toAdd, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes, long overridenDuration, uint overridenStackID)
        {
            if (!stacks.Any())
            {
                return false;
            }
            BuffStackItem toRemove = stacks[0];
            wastes.Add(new BuffSimulationItemWasted(toRemove.Src, toRemove.Duration, toRemove.Start));
            if (toRemove.Extensions.Count > 0)
            {
                foreach ((AgentItem src, long value) in toRemove.Extensions)
                {
                    wastes.Add(new BuffSimulationItemWasted(src, value, toRemove.Start));
                }
            }
            stacks.Remove(toRemove);
            Add(log, stacks, toAdd);
            return true;
        }

        // https://www.c-sharpcorner.com/blogs/binary-search-implementation-using-c-sharp1
        private static int BinarySearchRecursive(List<BuffStackItem> stacks, long totalDuration, int minIndex, int maxIndex)
        {
            if (stacks[minIndex].TotalDuration > totalDuration)
            {
                return minIndex;
            }
            if (stacks[maxIndex].TotalDuration < totalDuration)
            {
                return maxIndex + 1;
            }
            if (minIndex > maxIndex)
            {
                return minIndex;
            }
            else
            {
                int midIndex = (minIndex + maxIndex) / 2;
                if (totalDuration == stacks[midIndex].TotalDuration)
                {
                    return ++midIndex;
                }
                else if (totalDuration < stacks[midIndex].TotalDuration)
                {
                    return BinarySearchRecursive(stacks, totalDuration, minIndex, midIndex - 1);
                }
                else
                {
                    return BinarySearchRecursive(stacks, totalDuration, midIndex + 1, maxIndex);
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
