using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class BuffSimulationItemDuration : BuffSimulationItemStack
    {
        public BuffSimulationItemDuration(IReadOnlyList<BuffStackItem> stacks) : base(stacks)
        {
        }

        public override void OverrideEnd(long end)
        {
            Stacks[0].OverrideEnd(end);
            Duration = Stacks[0].Duration;
        }

        public override int GetActiveStacks()
        {
            return 1;
        }
        public override int GetActiveStacks(AbstractSingleActor actor)
        {
            return (GetSources().First() == actor.AgentItem) ? 1 : 0;
        }
        public override IEnumerable<AgentItem> GetActiveSources()
        {
            return GetSources().Take(1);
        }

        public override long GetActualDuration()
        {
            return GetActualDurationPerStack().Sum();
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid)
        {
            Stacks.First().SetBuffDistributionItem(distribs, start, end, boonid);
        }
    }
}
