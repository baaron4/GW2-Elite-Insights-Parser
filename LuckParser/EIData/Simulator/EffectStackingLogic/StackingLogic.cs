using System.Collections.Generic;
using LuckParser.Parser;
using static LuckParser.EIData.BoonSimulator;

namespace LuckParser.EIData
{
    public abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationItemWasted> wastes);

        public abstract void Sort(ParsedLog log, List<BoonStackItem> stacks);
    }
}
