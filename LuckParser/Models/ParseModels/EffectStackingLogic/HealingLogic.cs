using System.Collections.Generic;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class HealingLogic : QueueLogic
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
    }
}
