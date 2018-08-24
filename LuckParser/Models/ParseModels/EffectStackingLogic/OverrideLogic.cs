using System.Collections.Generic;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class OverrideLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonSimulator.BoonStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BoonSimulator.BoonStackItem toAdd, List<BoonSimulator.BoonStackItem> stacks, List<BoonSimulationItem> simulation)
        {
            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[i].BoonDuration < toAdd.BoonDuration)
                {
                    long overstackValue = stacks[i].Overstack + stacks[i].BoonDuration;
                    ushort srcValue = stacks[i].Src;
                    for (int j = simulation.Count - 1; j >= 0; j--)
                    {
                        if (simulation[j].AddOverstack(srcValue, overstackValue))
                        {
                            break;
                        }
                    }
                    stacks[i] = toAdd;
                    return true;
                }
            }
            return false;
        }
    }
}
