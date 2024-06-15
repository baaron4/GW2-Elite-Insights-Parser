using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal abstract class BuffSimulationItemStack : BuffSimulationItem
    {
        protected readonly BuffSimulationItemBase[] Stacks;
        private readonly AgentItem[] _sources;
        private Dictionary<AgentItem, int> _stacksPerSource { get; set; }

        public BuffSimulationItemStack(IReadOnlyList<BuffStackItem> stacks) : base(stacks.First().Start, stacks.First().Duration)
        {
            int count = stacks.Count;
            _sources = new AgentItem[count];
            Stacks = new BuffSimulationItemBase[count];
            for (int i = 0; i < count; i++)
            {
                BuffStackItem stackItem = stacks[i];
                Stacks[i] = new BuffSimulationItemBase(stackItem);
                _sources[i] = stackItem.Src;
            }
        }
        public override int GetStacks()
        {
            return Stacks.Length;
        }

        public override int GetStacks(AbstractSingleActor actor)
        {
            if (_stacksPerSource == null)
            {
                _stacksPerSource = _sources.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            }
            if (_stacksPerSource.TryGetValue(actor.AgentItem, out var stacks))
            {
                return stacks;
            }
            return 0;
        }

        public override IReadOnlyList<long> GetActualDurationPerStack()
        {
            return new List<long>(Stacks.Select(x => x.GetActualDuration()));
        }

        public override IReadOnlyList<AgentItem> GetSources()
        {
            return _sources;
        }
    }
}
