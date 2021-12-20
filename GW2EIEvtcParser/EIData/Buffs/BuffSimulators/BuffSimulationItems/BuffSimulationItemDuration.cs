using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class BuffSimulationItemDuration : BuffSimulationItemStack
    {
        public BuffSimulationItemDuration(IEnumerable<BuffStackItem> stacks) : base(stacks)
        {
        }

        public override void OverrideEnd(long end)
        {
            Stacks.First().OverrideEnd(end);
            Duration = Stacks.First().Duration;
        }

        public override int GetActiveStacks()
        {
            return 1;
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid)
        {
            Stacks.First().SetBuffDistributionItem(distribs, start, end, boonid);
        }
    }
}
