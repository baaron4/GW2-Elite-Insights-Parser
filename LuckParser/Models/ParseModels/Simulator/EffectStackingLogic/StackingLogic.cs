using LuckParser.Models.DataModels;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationItemOverstack> overstacks);

        public abstract void Sort(ParsedLog log, List<BoonStackItem> stacks);
    }
}
