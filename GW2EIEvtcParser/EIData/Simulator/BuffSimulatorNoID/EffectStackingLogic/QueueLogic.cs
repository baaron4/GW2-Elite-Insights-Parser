using System.Collections.Generic;
using System.IO;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal class QueueLogic : StackingLogic
    {
        public override void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedEvtcLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes)
        {
            if (stacks.Count <= 1)
            {
                throw new InvalidDataException("Queue logic based must have a >1 capacity");
            }
            BuffStackItem first = stacks[0];
            stacks.RemoveAt(0);
            BuffStackItem minItem = stacks.MinBy(x => x.TotalDuration);
            if (minItem.TotalDuration > stackItem.TotalDuration + ParserHelper.BuffSimulatorDelayConstant)
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
