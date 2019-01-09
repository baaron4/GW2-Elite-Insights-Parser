using System.Collections.Generic;
using LuckParser.Parser;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class OverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            stacks.Sort((x, y) => x.TotalBoonDuration().CompareTo(y.TotalBoonDuration()));
        }

        public override bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonSimulator.BoonStackItem> stacks, List<BoonSimulationItemWasted> wastes)
        {
            if (stacks.Count == 0)
            {
                return false;
            }
            BoonStackItem stack = stacks[0];
            if (stack.TotalBoonDuration() < stackItem.TotalBoonDuration())
            {
                wastes.Add(new BoonSimulationItemWasted(stack.Src, stack.BoonDuration, stack.Start, stack.ApplicationTime));
                if (stack.Extensions.Count > 0)
                {
                    foreach ((ushort src, long value, long time) in stack.Extensions)
                    {
                        wastes.Add(new BoonSimulationItemWasted(src, value, stack.Start, time));
                    }
                }
                stacks[0] = stackItem;
                Sort(log, stacks);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
