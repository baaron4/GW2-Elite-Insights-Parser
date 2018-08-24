using System.Collections.Generic;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class HealingLogic : StackingLogic
    {
        private struct CompareHealing
        {
            private readonly ParsedLog _log;

            public CompareHealing(ParsedLog log)
            {
                _log = log;
            }

            public int Compare(BoonSimulator.BoonStackItem x, BoonSimulator.BoonStackItem y)
            {
                List<Player> players = _log.GetPlayerList();
                Player a = players.Find(p => p.GetInstid() == x.Src);
                Player b = players.Find(p => p.GetInstid() == y.Src);
                if (a == null || b == null)
                {
                    return 0;
                }
                return a.GetHealing() < b.GetHealing() ? 1 : -1;
            }
        }
        public override void Sort(ParsedLog log, List<BoonSimulator.BoonStackItem> stacks)
        {
            CompareHealing comparator = new CompareHealing(log);
            stacks.Sort(comparator.Compare);        
        }

        public override bool StackEffect(ParsedLog log, BoonSimulator.BoonStackItem toAdd, List<BoonSimulator.BoonStackItem> stacks, List<BoonSimulationItem> simulation)
        {
            
            for (int i = 1; i < stacks.Count; i++)
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
                    Sort(log, stacks);
                    return true;
                }
            }
            return false;
        }
    }
}
