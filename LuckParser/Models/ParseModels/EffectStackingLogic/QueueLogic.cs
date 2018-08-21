using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class QueueLogic : StackingLogic
    {
        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            // no sort
        }

        public override bool StackEffect(ParsedLog log, BoonStackItem toAdd, List<BoonStackItem> stacks, List<BoonSimulationItem> simulation)
        {
            for (int i = 1; i < stacks.Count; i++)
            {
                if (stacks[i].boon_duration < toAdd.boon_duration)
                {
                    long overstackValue = stacks[i].overstack + stacks[i].boon_duration;
                    ushort srcValue = stacks[i].src;
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
