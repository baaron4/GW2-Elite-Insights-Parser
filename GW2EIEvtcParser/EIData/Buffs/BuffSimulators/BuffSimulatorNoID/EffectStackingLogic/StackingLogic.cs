using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal abstract class StackingLogic
    {
        public abstract bool FindLowestValue(ParsedEvtcLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes, long overridenDuration, uint overridenStackID);

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

        public virtual void Activate(List<BuffStackItem> stacks, uint stackID)
        {

        }

        public virtual void Activate(List<BuffStackItem> stacks, BuffStackItem stackItem)
        {

        }

        public virtual void Reset(List<BuffStackItem> stacks, uint id, long toDuration)
        {

        }

    }
}
