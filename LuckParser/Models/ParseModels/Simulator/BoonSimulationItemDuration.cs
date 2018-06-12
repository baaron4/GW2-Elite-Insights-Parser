using System;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemDuration: BoonSimulationItem
    {
        private ushort src;
        private long overstack;

        public BoonSimulationItemDuration(long start, long duration, ushort src, long overstack) : base(start, duration)
        {
            this.src = src;
            this.overstack = overstack;
        }

        public BoonSimulationItemDuration(BoonStackItem other) : base(other.start, other.boon_duration)
        {
            this.src = other.src;
            this.overstack = other.overstack;
        }

        public override long getDuration(ushort src)
        {
            return this.duration;
        }
        public override void setEnd(long end)
        {
            this.duration = Math.Min(Math.Max(end - this.start, 0),duration);
        }

        public override List<ushort> getSrc()
        {
            List<ushort> res = new List<ushort>
            {
                src
            };
            return res;
        }

        public override int getStack(long end)
        {
            return 1;
        }

        public override long getOverstack(ushort src)
        {
            return overstack;
        }

        public override bool addOverstack(ushort src, long overstack)
        {
            if (this.src != src || duration == 0)
            {
                return false;
            }
            this.overstack += overstack;
            return true;
        }
    }
}
