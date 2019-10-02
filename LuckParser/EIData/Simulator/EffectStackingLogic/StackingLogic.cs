using System.Collections.Generic;
using LuckParser.Parser;
using static LuckParser.EIData.BuffSimulator;

namespace LuckParser.EIData
{
    public abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes);

        public abstract void Sort(ParsedLog log, List<BuffStackItem> stacks);
    }
}
