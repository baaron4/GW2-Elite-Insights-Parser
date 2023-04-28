using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class BuffSimulationItemIntensity : BuffSimulationItemStack
    {
        public BuffSimulationItemIntensity(IEnumerable<BuffStackItem> stacks) : base(stacks)
        {
            Duration = Stacks.Max(x => x.Duration);
        }

        public override void OverrideEnd(long end)
        {
            foreach (BuffSimulationItemBase stack in Stacks)
            {
                stack.OverrideEnd(end);
            }
            Duration = Stacks.Max(x => x.Duration);
        }

        public override int GetActiveStacks()
        {
            return Stacks.Count;
        }

        public override IReadOnlyList<AgentItem> GetActiveSources()
        {
            return GetSources();
        }

        public override int GetActiveStacks(AbstractSingleActor actor)
        {
            return GetActiveSources(actor).Count;
        }

        public override IReadOnlyList<AgentItem> GetActiveSources(AbstractSingleActor actor)
        {
            return GetSources(actor);
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid)
        {
            long cDur = GetClampedDuration(start, end);
            if (cDur == 0)
            {
                return;
            }
            foreach (BuffSimulationItemBase item in Stacks)
            {
                item.SetBuffDistributionItem(distribs, start, end, boonid);
            }
        }
    }
}
