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

        public override bool StackEffect(ParsedLog log, BoonSimulator.BoonStackItem stackItem, List<BoonSimulator.BoonStackItem> stacks, List<BoonSimulationItem> simulation)
        {
            if (stacks.Count == 0)
            {
                return false;
            }
            long overstackValue = stacks[0].Overstack + stacks[0].BoonDuration;
            ushort srcValue = stacks[0].Src;
            if (simulation.Count == 0)
            {
                simulation.Add(new BoonSimulationOverstackItem(new BoonStackItem(stacks[0].Start, 1, srcValue, overstackValue)));
            }
            else
            {
                simulation.Insert(simulation.Count - 1, new BoonSimulationOverstackItem(new BoonStackItem(stacks[0].Start, 1, srcValue, overstackValue)));
            }
            stacks[0] = stackItem;
            return true;
        }
    }
}
