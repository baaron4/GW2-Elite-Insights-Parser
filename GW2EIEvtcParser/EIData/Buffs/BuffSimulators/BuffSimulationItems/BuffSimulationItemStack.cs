using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal abstract class BuffSimulationItemStack : BuffSimulationItem
    {
        protected readonly List<BuffSimulationItemBase> Stacks = new List<BuffSimulationItemBase>();
        private readonly List<AgentItem> _sources;

        public BuffSimulationItemStack(IEnumerable<BuffStackItem> stacks) : base(stacks.First().Start, stacks.First().Duration)
        {
            foreach (BuffStackItem stack in stacks)
            {
                Stacks.Add(new BuffSimulationItemBase(stack));
            }
            _sources = new List<AgentItem>();
            foreach (BuffSimulationItemBase item in Stacks)
            {
                _sources.AddRange(item.GetSources());
            }
        }
        public override int GetStacks()
        {
            return Stacks.Count;
        }

        public override int GetStacks(AbstractSingleActor actor)
        {
            return GetSources(actor).Count;
        }

        public override IReadOnlyList<long> GetActualDurationPerStack()
        {
            return new List<long>(Stacks.Select(x => x.GetActualDuration()));
        }

        public override IReadOnlyList<AgentItem> GetSources()
        {
            return _sources;
        }

        public override IReadOnlyList<AgentItem> GetSources(AbstractSingleActor actor)
        {
            return _sources.Where(x => x == actor.AgentItem).ToList();
        }
    }
}
