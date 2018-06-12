using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulationItem
    {
        protected long start;
        protected long duration;

        public BoonSimulationItem()
        {
            this.start = 0;
            this.duration = 0;
        }

        public BoonSimulationItem(long start, long duration)
        {
            this.start = start;
            this.duration = duration;
        }

        public abstract long getDuration(ushort src);


        public long getStart()
        {
            return start;
        }

        public abstract List<ushort> getSrc();

        public long getEnd()
        {
            return start + duration;
        }

        public abstract bool addOverstack(ushort src, long overstack);

        public abstract long getOverstack(ushort src);

        public long getItemDuration()
        {
            return duration;
        }

        public abstract void setEnd(long end);

        public abstract int getStack(long end);
    }
}
