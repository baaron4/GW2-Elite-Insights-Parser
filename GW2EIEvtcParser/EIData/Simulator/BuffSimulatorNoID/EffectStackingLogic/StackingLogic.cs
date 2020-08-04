using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    public abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedEvtcLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes);

        public abstract void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks);
    }
}
