using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class OverrideLogic : StackingLogic
    {
        public override void sort(ParsedLog log, List<BoonSimulator.BoonStackItem> stacks)
        {
            // no sort
        }

        public override bool stackEffect(ParsedLog log, BoonSimulator.BoonStackItem toAdd, List<BoonSimulator.BoonStackItem> stacks, List<BoonSimulationItem> simulation)
        {
            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[i].boon_duration < toAdd.boon_duration)
                {
                    long overstackValue = stacks[i].overstack + stacks[i].boon_duration;
                    ushort srcValue = stacks[i].src;
                    for (int j = simulation.Count - 1; j >= 0; j--)
                    {
                        if (simulation[j].addOverstack(srcValue, overstackValue))
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
