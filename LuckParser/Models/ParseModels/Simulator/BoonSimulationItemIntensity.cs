using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    class BoonSimulationItemIntensity : BoonSimulationItem
    {
        private List<BoonSimulationItemDuration> stacks = new List<BoonSimulationItemDuration>();

        public BoonSimulationItemIntensity(List<BoonStackItem> stacks) : base()
        {
            this.start = stacks[0].start;
            foreach (BoonStackItem stack in stacks)
            {
                this.stacks.Add(new BoonSimulationItemDuration(stack));
            }
            this.duration = this.stacks.Max(x => x.GetDuration(0));
        }

        public override void SetEnd(long end)
        {
            foreach (BoonSimulationItemDuration stack in stacks)
            {
                stack.SetEnd(end);
            }
            this.duration = this.stacks.Max(x => x.GetDuration(0));
        }

        public override long GetDuration(ushort src, long start = 0, long end = 0)
        {
            long total = 0;
            if (src == 0)
            {
                foreach (BoonSimulationItemDuration stack in stacks)
                {
                    total += stack.GetDuration(src, start, end);
                }
            } else
            {

                foreach (BoonSimulationItemDuration stack in stacks.Where(x => x.GetSrc()[0] == src))
                {
                    total += stack.GetDuration(src, start, end);
                }
            }
            return total;
        }
        
        public override List<ushort> GetSrc()
        {
            return stacks.Select(x => x.GetSrc()[0]).Distinct().ToList();
        }

        public override int GetStack(long end)
        {
            return stacks.Count(x => x.GetEnd() >= end);
        }

        public override long GetOverstack(ushort src, long start = 0, long end = 0)
        {
            long total = 0;
            if (src == 0)
            {
                foreach (BoonSimulationItemDuration stack in stacks)
                {
                    total += stack.GetOverstack(src, start, end);
                }
            }
            else
            {

                foreach (BoonSimulationItemDuration stack in stacks.Where(x => x.GetSrc()[0] == src))
                {
                    total += stack.GetOverstack(src, start, end);
                }
            }
            return total;
        }

        public override bool AddOverstack(ushort src, long overstack)
        {
            if (duration == 0)
            {
                return false;
            }
            foreach(BoonSimulationItemDuration stack in stacks)
            {
                if (stack.AddOverstack(src,overstack))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
