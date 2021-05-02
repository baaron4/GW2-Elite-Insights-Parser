using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class StackingLogic
    {
        public abstract bool FindLowestValue(ParsedEvtcLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes);

        public virtual bool IsFull(List<BuffStackItem> stacks, int capacity)
        {
            return stacks.Count == capacity;
        }

        protected abstract void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks);
        public virtual void Add(ParsedEvtcLog log, List<BuffStackItem> stacks, BuffStackItem stackItem)
        {
            stacks.Add(stackItem);
            Sort(log, stacks);
        }

    }
}
