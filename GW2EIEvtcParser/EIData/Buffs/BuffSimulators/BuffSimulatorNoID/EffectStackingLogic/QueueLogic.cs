using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class QueueLogic : StackingLogic
    {
        protected override void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks)
        {
            // nothign to sort
        }

        public override bool FindLowestValue(ParsedEvtcLog log, BuffStackItem toAdd, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes, long overridenDuration, uint overridenStackID)
        {
            if (stacks.Count <= 1)
            {
                throw new InvalidDataException("Queue logic based must have a >1 capacity");
            }
            BuffStackItem first = stacks[0];
            BuffStackItem toRemove = stacks.Where(x => x != first).MinBy(x => x.TotalDuration);
            wastes.Add(new BuffSimulationItemWasted(toRemove.Src, toRemove.Duration, toRemove.Start));
            if (toRemove.Extensions.Any())
            {
                foreach ((AgentItem src, long value) in toRemove.Extensions)
                {
                    wastes.Add(new BuffSimulationItemWasted(src, value, toRemove.Start));
                }
            }
            stacks[stacks.IndexOf(toRemove)] = toAdd;
            Sort(log, stacks);
            return true;
        }
        public override void Activate(List<BuffStackItem> stacks, BuffStackItem stackItem)
        {
            stacks.Remove(stackItem);
            stacks.Insert(0, stackItem);
        }
    }
}
