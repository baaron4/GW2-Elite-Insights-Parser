using System;
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

        public abstract long GetDuration(ushort src, long start = 0, long end = 0);


        public long GetStart()
        {
            return start;
        }

        public abstract List<ushort> GetSrc();

        public long GetEnd()
        {
            return start + duration;
        }

        public abstract bool AddOverstack(ushort src, long overstack);

        public abstract long GetOverstack(ushort src, long start = 0, long end = 0);

        public long GetItemDuration(long start = 0, long end = 0)
        {
            if (end > 0)
            {
                long start_offset = Math.Max(Math.Min(duration, start - this.start),0);
                long item_end = this.start + duration;
                long end_offset = Math.Max(Math.Min(duration, item_end - end),0);
                return duration - start_offset - end_offset;
            }
            return duration;
        }

        public abstract void SetEnd(long end);

        public abstract int GetStack(long end);
    }
}
