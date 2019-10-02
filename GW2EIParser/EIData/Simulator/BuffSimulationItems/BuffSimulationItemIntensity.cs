using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemIntensity : BuffSimulationItem
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

        public override void SetBoonDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            foreach (BuffSimulationItemDuration item in _stacks)
            {
                item.SetBoonDistributionItem(distribs, start, end, boonid, log);
            }
        }

        public override List<AgentItem> GetSources()
        {
            return _sources;
        }
    }
}
