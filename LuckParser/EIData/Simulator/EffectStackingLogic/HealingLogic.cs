using System.Collections.Generic;
using LuckParser.Parser;
using static LuckParser.EIData.BoonSimulator;

namespace LuckParser.EIData
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

            private uint GetHealing(BoonStackItem stack)
            {
                return stack.SeedSrc.Healing;
            }

            public int Compare(BoonStackItem x, BoonStackItem y)
            {             
                return -GetHealing(x).CompareTo(GetHealing(y));
            }
        }

        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            CompareHealing comparator = new CompareHealing(log);
            stacks.Sort(comparator.Compare);        
        }
    }
}
