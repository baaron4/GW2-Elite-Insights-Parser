using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal class CappedDurationLogic : StackingLogic
    {
        protected override void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks)
        {
            // nothing to sort
        }

        public override bool IsFull(List<BuffStackItem> stacks, int capacity)
        {
            // never full
            return false;
        }

        public override bool FindLowestValue(ParsedEvtcLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes)
        {
            // no lowest value to find
            return false;
        }
    }
}
