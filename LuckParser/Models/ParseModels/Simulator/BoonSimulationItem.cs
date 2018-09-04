using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulationItem
    {
        public long Start { get; protected set; }
        public long Duration { get; protected set; }
        public long End
        {
            get
            {
                return Start + Duration;
            }
        }

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

        public abstract long GetSrcDuration(ushort src, long start, long end);
        public abstract long GetItemDuration();

        public abstract List<ushort> GetSrc();
        
        public abstract bool AddOverstack(ushort src, long overstack);

        public abstract long GetOverstack(ushort src, long start, long end);

        public virtual long GetClampedDuration(long start, long end)
        {
            if (end > 0 && end - start > 0)
            {
                long startoffset = Math.Max(Math.Min(Duration, start - Start),0);
                long itemEnd = Start + Duration;
                long endOffset = Math.Max(Math.Min(Duration, itemEnd - end),0);
                return Duration - startoffset - endOffset;
            }
            return 0;
        }

        public abstract List<BoonsGraphModel.Segment> ToSegment();

        public abstract void SetEnd(long end);

        public abstract int GetStack(long end);
    }
}
