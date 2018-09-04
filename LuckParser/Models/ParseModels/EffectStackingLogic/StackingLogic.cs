using LuckParser.Models.DataModels;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public abstract class StackingLogic
    {
        public abstract bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationItem> simulation);

        protected bool StackEffect(int startIndex, ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationItem> simulation)
        {
            for (int i = startIndex; i < stacks.Count; i++)
            {
                if (stacks[i].BoonDuration < stackItem.BoonDuration)
                {
                    long overstackValue = stacks[i].Overstack + stacks[i].BoonDuration;
                    ushort srcValue = stacks[i].Src;
                    bool found = false;
                    for (int j = simulation.Count - 1; j >= 0; j--)
                    {
                        if (simulation[j].AddOverstack(srcValue, overstackValue))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        if (simulation.Count == 0)
                        {
                            simulation.Add(new BoonSimulationOverstackItem(new BoonStackItem(stacks[i].Start, 1, srcValue, overstackValue)));
                        }
                        else
                        {
                            simulation.Insert(simulation.Count - 1, new BoonSimulationOverstackItem(new BoonStackItem(stacks[i].Start, 1, srcValue, overstackValue)));
                        }
                    }
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
