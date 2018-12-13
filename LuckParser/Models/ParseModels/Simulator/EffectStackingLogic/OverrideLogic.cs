using System.Collections.Generic;
using LuckParser.Models.DataModels;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class OverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonSimulator.BoonStackItem> stacks)
        {
            stacks.Sort((x, y) => x.BoonDuration.CompareTo(y.BoonDuration));
        }

        public override bool StackEffect(ParsedLog log, BoonSimulator.BoonStackItem stackItem, List<BoonSimulator.BoonStackItem> stacks, List<BoonSimulationOverstackItem> overstacks)
        {
            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[i].BoonDuration < stackItem.BoonDuration)
                {
                    BoonStackItem stack = stacks[i];
                    overstacks.Add(new BoonSimulationOverstackItem(stack.Src, stack.BoonDuration, stack.Start));
                    stacks[i] = stackItem;
                    Sort(log, stacks);
                    return true;
                }
            }
            return false;
        }
    }
}
