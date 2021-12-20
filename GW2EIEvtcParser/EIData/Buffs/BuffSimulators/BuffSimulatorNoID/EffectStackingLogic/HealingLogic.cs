using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class HealingLogic : QueueLogic
    {

        private struct CompareHealing
        {

            private static uint GetHealing(BuffStackItem stack)
            {
                return stack.SeedSrc.Healing;
            }

            public static int Compare(BuffStackItem x, BuffStackItem y)
            {
                return -GetHealing(x).CompareTo(GetHealing(y));
            }
        }

        protected override void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks)
        {
            stacks.Sort(CompareHealing.Compare);
        }
    }
}
