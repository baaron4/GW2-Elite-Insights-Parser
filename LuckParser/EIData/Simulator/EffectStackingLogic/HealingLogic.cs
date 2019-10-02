using System.Collections.Generic;
using LuckParser.Parser;
using static LuckParser.EIData.BuffSimulator;

namespace LuckParser.EIData
{
    public class HealingLogic : QueueLogic
    {

        private struct CompareHealing
        {

            private static uint GetHealing(BuffStackItem stack)
            {
                return stack.SeedSrc.Healing;
            }

            public int Compare(BuffStackItem x, BuffStackItem y)
            {
                return -GetHealing(x).CompareTo(GetHealing(y));
            }
        }

        public override void Sort(ParsedLog log, List<BuffStackItem> stacks)
        {
            var comparator = new CompareHealing();
            stacks.Sort(comparator.Compare);
        }
    }
}
