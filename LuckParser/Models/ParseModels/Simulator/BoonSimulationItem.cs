using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulationItem
    {
        protected long Start;
        protected long Duration;

        protected BoonSimulationItem()
        {
            Start = 0;
            Duration = 0;
        }

        protected BoonSimulationItem(long start, long duration)
        {
            Start = start;
            Duration = duration;
        }

        public abstract long GetDuration(ushort src, long start = 0, long end = 0);
        public abstract long GetDuration(long start = 0, long end = 0);


        public long GetStart()
        {
            return Start;
        }

        public abstract List<ushort> GetSrc();

        public long GetEnd()
        {
            return Start + Duration;
        }

        public abstract bool AddOverstack(ushort src, long overstack);

        public abstract long GetOverstack(ushort src, long start = 0, long end = 0);

        public virtual long GetItemDuration(long start = 0, long end = 0)
        {
            if (end > 0)
            {
                long startoffset = Math.Max(Math.Min(Duration, start - Start),0);
                long itemEnd = Start + Duration;
                long endOffset = Math.Max(Math.Min(Duration, itemEnd - end),0);
                return Duration - startoffset - endOffset;
            }
            return Duration;
        }

        public abstract List<BoonsGraphModel.Segment> ToSegment();

        public abstract void SetEnd(long end);

        public abstract int GetStack(long end);
    }
}
