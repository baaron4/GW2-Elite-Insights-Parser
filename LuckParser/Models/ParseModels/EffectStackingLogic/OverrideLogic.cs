using System.Collections.Generic;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class OverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonSimulator.BoonStackItem> stacks)
        {
            stacks.Sort((x, y) => x.BoonDuration < y.BoonDuration ? -1 : 1);
        }

        public override bool StackEffect(ParsedLog log, BoonSimulator.BoonStackItem toAdd, List<BoonSimulator.BoonStackItem> stacks, List<BoonSimulationItem> simulation)
        {
            return StackEffect(0, log, toAdd, stacks, simulation);
        }
    }
}
