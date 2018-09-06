using LuckParser.Models.DataModels;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationOverstackItem> overstacks);

        protected bool StackEffect(int startIndex, ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationOverstackItem> overstacks)
        {
            for (int i = startIndex; i < stacks.Count; i++)
            {
                if (stacks[i].InitialBoonDuration < stackItem.InitialBoonDuration)
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

        public abstract void Sort(ParsedLog log, List<BoonStackItem> stacks);
    }
}
