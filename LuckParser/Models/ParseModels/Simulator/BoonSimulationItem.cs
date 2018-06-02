using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItem
    {
        private long start;
        private long duration;
        private ushort src;

        public BoonSimulationItem(long start, long duration, ushort src)
        {
            this.start = start;
            this.duration = duration;
            this.src = src;
        }

        public BoonSimulationItem(BoonSimulationItem other, long start_shift, long duration_shift)
        {
            this.start = other.start + start_shift;
            this.duration = other.duration + duration_shift;
            this.src = other.src;
        }

        public long getDuration()
        {
            return this.duration;
        }

        public void shiftStart(long shift)
        {
            start += shift;
        }

        public long getStart()
        {
            return start;
        }

        public void setDuration(long duration)
        {
            this.duration = duration;
        }

        public ushort getSrc()
        {
            return src;
        }

        public void setEnd(long end)
        {
            this.duration = end - this.start;
        }

        public long getEnd()
        {
            return start + duration;
        }
    }
}
