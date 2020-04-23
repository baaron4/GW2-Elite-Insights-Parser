using System;
using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.EIData.AbstractBuffSimulator;

namespace GW2EIParser.EIData
{
    public class QueueLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BuffStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes)
        {
            if (stacks.Count <= 1)
            {
                throw new InvalidOperationException("Queue logic based must have a >1 capacity");
            }
            BuffStackItem first = stacks[0];
            stacks.RemoveAt(0);
            BuffStackItem minItem = stacks.MinBy(x => x.TotalBoonDuration());
            if (minItem.TotalBoonDuration() > stackItem.TotalBoonDuration() + GeneralHelper.ServerDelayConstant)
            {
                stacks.Insert(0, first);
                return false;
            }
            wastes.Add(new BuffSimulationItemWasted(minItem.Src, minItem.Duration, minItem.Start));
            if (minItem.Extensions.Count > 0)
            {
                foreach ((AgentItem src, long value) in minItem.Extensions)
                {
                    wastes.Add(new BuffSimulationItemWasted(src, value, minItem.Start));
                }
            }
            stacks[stacks.IndexOf(minItem)] = stackItem;
            stacks.Insert(0, first);
            Sort(log, stacks);
            return true;
        }
    }
}
