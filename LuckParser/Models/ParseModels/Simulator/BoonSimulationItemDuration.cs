using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemDuration: BoonSimulationItem
    {
        private long start;
        private long duration;
        private ushort src;

        public BoonSimulationItemDuration(long start, long duration, ushort src) : base()
        {
            this.start = start;
            this.duration = duration;
            this.src = src;
        }

        public BoonSimulationItemDuration(BoonStackItem other) : base()
        {
            this.start = other.start ;
            this.duration = other.boon_duration;
            this.src = other.src;
        }

        public override long getDuration()
        {
            return this.duration;
        }

        public override long getStart()
        {
            return start;
        }

        public void setDuration(long duration)
        {
            this.duration = duration;
        }

        public override ushort getSrc()
        {
            return src;
        }

        public void setEnd(long end)
        {
            this.duration = Math.Max(end - this.start, 0);
        }

        public override long getEnd()
        {
            return start + duration;
        }

        public override int getStack()
        {
            return 1;
        }
    }
}
