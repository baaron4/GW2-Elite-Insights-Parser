using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulationItemIntensity : BuffSimulationItem
    {
        private readonly List<BuffSimulationItemDuration> _stacks = new List<BuffSimulationItemDuration>();
        private readonly List<AgentItem> _sources;

        public BuffSimulationItemIntensity(List<BuffStackItem> stacks) : base(stacks[0].Start, 0)
        {
            foreach (BuffStackItem stack in stacks)
            {
                _stacks.Add(new BuffSimulationItemDuration(stack));
            }
            Duration = _stacks.Max(x => x.Duration);
            _sources = new List<AgentItem>();
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                _sources.AddRange(item.GetSources());
            }
        }

        public override void OverrideEnd(long end)
        {
            foreach (BuffSimulationItemDuration stack in _stacks)
            {
                stack.OverrideEnd(end);
            }
            Duration = _stacks.Max(x => x.Duration);
        }

        public override int GetStack()
        {
            return _stacks.Count;
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid)
        {
            long cDur = GetClampedDuration(start, end);
            if (cDur == 0)
            {
                return;
            }
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                item.SetBuffDistributionItem(distribs, start, end, boonid);
            }
        }

        public override List<AgentItem> GetSources()
        {
            return _sources;
        }
    }
}
