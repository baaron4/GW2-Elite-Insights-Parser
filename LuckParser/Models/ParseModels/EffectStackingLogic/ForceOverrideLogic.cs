using System.Collections.Generic;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class ForceOverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonSimulator.BoonStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BoonSimulator.BoonStackItem stackItem, List<BoonSimulator.BoonStackItem> stacks, List<BoonSimulationItem> simulation)
        {
            if (stacks.Count == 0)
            {
                return false;
            }
            long overstackValue = stacks[0].Overstack + stacks[0].BoonDuration;
            ushort srcValue = stacks[0].Src;
            for (int j = simulation.Count - 1; j >= 0; j--)
            {
                if (simulation[j].AddOverstack(srcValue, overstackValue))
                {
                    break;
                }
            }
            stacks[0] = stackItem;
            return true;
        }
    }
}
