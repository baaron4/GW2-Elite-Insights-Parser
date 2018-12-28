using System.Collections.Generic;
using LuckParser.Models.DataModels;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class OverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            stacks.Sort((x, y) => x.BoonDuration.CompareTo(y.BoonDuration));
        }

        public override bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonSimulator.BoonStackItem> stacks, List<BoonSimulationItemWasted> wastes)
        {
            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[i].BoonDuration < stackItem.BoonDuration)
                {
                    BoonStackItem stack = stacks[i];
                    wastes.Add(new BoonSimulationItemWasted(stack.Src, stack.BoonDuration, stack.Start, stack.ApplicationTime));
                    if (stack.Extensions.Count > 0)
                    {
                        foreach (var item in stack.Extensions)
                        {
                            wastes.Add(new BoonSimulationItemWasted(item.Item1, item.Item2, stack.Start, item.Item3));
                        }
                    }
                    stacks[i] = stackItem;
                    Sort(log, stacks);
                    return true;
                }
            }
            return false;
        }
    }
}
