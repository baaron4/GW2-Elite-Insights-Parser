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
            this.duration = this.stacks.Max(x => x.getDuration(0));
        }

        public override void setEnd(long end)
        {
            foreach (BoonSimulationItemDuration stack in stacks)
            {
                stack.setEnd(end);
            }
            this.duration = this.stacks.Max(x => x.getDuration(0));
        }

        public override long getDuration(ushort src, long start = 0, long end = 0)
        {
            long total = 0;
            if (src == 0)
            {
                foreach (BoonSimulationItemDuration stack in stacks)
                {
                    total += stack.getDuration(src, start, end);
                }
            } else
            {

                foreach (BoonSimulationItemDuration stack in stacks.Where(x => x.getSrc()[0] == src))
                {
                    total += stack.getDuration(src, start, end);
                }
            }
            return total;
        }
        
        public override List<ushort> getSrc()
        {
            return stacks.Select(x => x.getSrc()[0]).Distinct().ToList();
        }

        public override int getStack(long end)
        {
            return stacks.Count(x => x.getEnd() >= end);
        }

        public override long getOverstack(ushort src, long start = 0, long end = 0)
        {
            long total = 0;
            if (src == 0)
            {
                foreach (BoonSimulationItemDuration stack in stacks)
                {
                    total += stack.getOverstack(src, start, end);
                }
            }
            else
            {

                foreach (BoonSimulationItemDuration stack in stacks.Where(x => x.getSrc()[0] == src))
                {
                    total += stack.getOverstack(src, start, end);
                }
            }
            return total;
        }

        public override bool addOverstack(ushort src, long overstack)
        {
            if (duration == 0)
            {
                return false;
            }
            foreach(BoonSimulationItemDuration stack in stacks)
            {
                if (stack.addOverstack(src,overstack))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
