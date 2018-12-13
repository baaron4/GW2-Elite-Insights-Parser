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
                List<Player> players = _log.PlayerList;
                Player a = players.Find(p => p.InstID == x.OriginalSrc);
                Player b = players.Find(p => p.InstID == y.OriginalSrc);
                if (a == null || b == null)
                {
                    return 0;
                }
                return -a.Healing.CompareTo(b.Healing);
            }
        }
        public override void Sort(ParsedLog log, List<BoonSimulator.BoonStackItem> stacks)
        {
            CompareHealing comparator = new CompareHealing(log);
            stacks.Sort(comparator.Compare);        
        }
    }
}
