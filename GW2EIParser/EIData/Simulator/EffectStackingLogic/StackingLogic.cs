using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.EIData.BuffSimulator;

namespace GW2EIParser.EIData
{
    public abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes);

        public abstract void Sort(ParsedLog log, List<BuffStackItem> stacks);
    }
}
