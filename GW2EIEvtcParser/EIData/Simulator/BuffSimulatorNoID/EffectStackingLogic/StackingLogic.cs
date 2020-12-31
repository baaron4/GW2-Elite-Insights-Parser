using System;
using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class StackingLogic
    {
        private int _capacity { get; }

        protected StackingLogic(int capacity)
        {
            _capacity = Math.Max(capacity, 1);
        }

        public bool IsFull(List<BuffStackItem> stacks)
        {
            return _capacity == stacks.Count;
        }

        public abstract bool StackEffect(ParsedEvtcLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes);

        protected abstract void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks);
        public virtual void Add(ParsedEvtcLog log, List<BuffStackItem> stacks, BuffStackItem stackItem)
        {
            stacks.Add(stackItem);
            Sort(log, stacks);
        }

    }
}
