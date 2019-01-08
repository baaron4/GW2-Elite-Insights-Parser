using LuckParser.Parser;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationItemWasted> wastes);

        public abstract void Sort(ParsedLog log, List<BoonStackItem> stacks);
    }
}
