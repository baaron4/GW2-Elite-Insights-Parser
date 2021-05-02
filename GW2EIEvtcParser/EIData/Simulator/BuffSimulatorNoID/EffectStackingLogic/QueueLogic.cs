using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal class QueueLogic : StackingLogic
    {
        protected override void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks)
        {
            // if there are no elements nothing to sort
            // 1 element - is already sorted
            // 2 elements - is already sorted as the ticking stack can't be disturbed
            if (stacks.Count > 2)
            {
                BuffStackItem first = stacks.First();
                stacks.Sort((x, y) =>
                {
                    if (x == first)
                    {
                        return -1;
                    }
                    if (y == first)
                    {
                        return 1;
                    }
                    return -x.TotalDuration.CompareTo(y.TotalDuration);
                });
            }
        }

        public override bool FindLowestValue(ParsedEvtcLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes)
        {
            if (stacks.Count <= 1)
            {
                throw new InvalidDataException("Queue logic based must have a >1 capacity");
            }
            BuffStackItem first = stacks[0];
            BuffStackItem minItem = stacks.Where(x => x != first).MinBy(x => x.TotalDuration);
            if (minItem.TotalDuration > stackItem.TotalDuration + ParserHelper.BuffSimulatorDelayConstant)
            {
                return false;
            }
            wastes.Add(new BuffSimulationItemWasted(minItem.Src, minItem.Duration, minItem.Start));
            if (minItem.Extensions.Any())
            {
                foreach ((AgentItem src, long value) in minItem.Extensions)
                {
                    wastes.Add(new BuffSimulationItemWasted(src, value, minItem.Start));
                }
            }
            stacks[stacks.IndexOf(minItem)] = stackItem;
            Sort(log, stacks);
            return true;
        }
    }
}
