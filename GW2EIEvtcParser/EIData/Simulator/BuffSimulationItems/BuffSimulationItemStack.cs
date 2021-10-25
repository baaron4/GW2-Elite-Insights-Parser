using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class BuffSimulationItemStack : BuffSimulationItem
    {
        protected readonly List<BuffSimulationItemDuration> Stacks = new List<BuffSimulationItemDuration>();
        private readonly List<AgentItem> _sources;

        public BuffSimulationItemStack(IEnumerable<BuffStackItem> stacks) : base(stacks.First().Start, stacks.First().Duration)
        {
            foreach (BuffStackItem stack in stacks)
            {
                Stacks.Add(new BuffSimulationItemDuration(stack));
            }
            _sources = new List<AgentItem>();
            foreach (BuffSimulationItemDuration item in Stacks)
            {
                _sources.AddRange(item.GetSources());
            }
        }
        public override int GetStacks()
        {
            return Stacks.Count;
        }

        public override IReadOnlyList<long> GetActualDurationPerStack()
        {
            return new List<long>(Stacks.Select(x => x.GetActualDurationPerStack().First()));
        }

        public override List<AgentItem> GetSources()
        {
            return _sources;
        }
    }
}
