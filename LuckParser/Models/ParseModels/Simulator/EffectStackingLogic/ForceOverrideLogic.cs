using System.Collections.Generic;
using LuckParser.Models.DataModels;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class ForceOverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonSimulator.BoonStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationItemWasted> overstacks)
        {
            if (stacks.Count == 0)
            {
                return false;
            }
            BoonStackItem stack = stacks[0];
            overstacks.Add(new BoonSimulationItemWasted(stack.Src, stack.BoonDuration, stack.Start));
            stacks[0] = stackItem;
            return true;
        }
    }
}
