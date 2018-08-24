using LuckParser.Models.DataModels;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class QueueLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BoonStackItem toAdd, List<BoonStackItem> stacks, List<BoonSimulationItem> simulation)
        {
            return StackEffect(1, log, toAdd, stacks, simulation);
        }
    }
}
