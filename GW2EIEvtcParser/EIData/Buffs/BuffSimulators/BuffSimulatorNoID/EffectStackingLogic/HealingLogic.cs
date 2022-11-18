using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class HealingLogic : QueueLogic
    {
        private bool _noSort = false;
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
            if (_noSort)
            {
                return;
            }
            stacks.Sort(CompareHealing.Compare);
        }

        public override void Activate(List<BuffStackItem> stacks, uint id)
        {
            BuffStackItem toActivate = stacks.FirstOrDefault(x => x.StackID == id);
            if (toActivate != null){
                _noSort = true;
                stacks.Remove(toActivate);
                stacks.Insert(0, toActivate);
            }
        }
    }
}
