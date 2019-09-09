using System.Collections.Generic;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using static LuckParser.EIData.BoonSimulator;

namespace LuckParser.EIData
{
    public class OverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            stacks.Sort((x, y) => x.TotalBoonDuration().CompareTo(y.TotalBoonDuration()));
        }

        public override bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationItemWasted> wastes)
        {
            if (stacks.Count == 0)
            {
                return false;
            }
            BoonStackItem stack = stacks[0];
            if (stack.TotalBoonDuration() <= stackItem.TotalBoonDuration() + 10)
            {
                wastes.Add(new BoonSimulationItemWasted(stack.Src, stack.BoonDuration, stack.Start));
                if (stack.Extensions.Count > 0)
                {
                    foreach ((AgentItem src, long value) in stack.Extensions)
                    {
                        wastes.Add(new BoonSimulationItemWasted(src, value, stack.Start));
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
