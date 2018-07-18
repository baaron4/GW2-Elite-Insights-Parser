using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class HealingLogic : StackingLogic
    {
        public override void sort(ParsedLog log, List<BoonSimulator.BoonStackItem> stacks)
        {
            List<Player> players = log.getPlayerList();
            stacks.Sort((x, y) => players.Find(p => p.getInstid() == x.src).getHealing() < players.Find(p => p.getInstid() == y.src).getHealing() ? -1 : 1);
        }

        public override bool stackEffect(ParsedLog log, BoonSimulator.BoonStackItem toAdd, List<BoonSimulator.BoonStackItem> stacks, List<BoonSimulationItem> simulation)
        {
            
            for (int i = 1; i < stacks.Count; i++)
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
                    sort(log, stacks);
                    return true;
                }
            }
            return false;
        }
    }
}
