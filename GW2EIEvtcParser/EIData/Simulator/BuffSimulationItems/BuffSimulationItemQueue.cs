using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulationItemQueue : BuffSimulationItemStack
    {
        public BuffSimulationItemQueue(IEnumerable<BuffStackItem> stacks) : base(stacks)
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
