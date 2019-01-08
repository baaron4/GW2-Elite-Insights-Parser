using System.Collections.Generic;
using LuckParser.Parser;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class ForceOverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonSimulator.BoonStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationItemWasted> wastes)
        {
            if (stacks.Count == 0)
            {
                return false;
            }
            BoonStackItem stack = stacks[0];
            wastes.Add(new BoonSimulationItemWasted(stack.Src, stack.BoonDuration, stack.Start, stack.ApplicationTime));
            if (stack.Extensions.Count > 0)
            {
                foreach (var item in stack.Extensions)
                {
                    wastes.Add(new BoonSimulationItemWasted(item.Item1, item.Item2, stack.Start, item.Item3));
                }
            }
            stacks[0] = stackItem;
            return true;
        }
    }
}
