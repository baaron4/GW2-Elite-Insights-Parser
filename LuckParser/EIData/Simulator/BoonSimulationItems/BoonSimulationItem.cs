using System;
using System.Collections.Generic;
using LuckParser.Parser.ParsedData;

namespace LuckParser.EIData
{
    public abstract class BoonSimulationItem : AbstractBoonSimulationItem
    {
        public long Duration { get; protected set; }
        public long Start { get; protected set; }
        public long End => Start + Duration;

        protected BoonSimulationItem(long start, long duration)
        {
            Start = start;
            Duration = duration;
        }

        public long GetClampedDuration(long start, long end)
        {
            if (end > 0 && end - start > 0)
            {
                long startoffset = Math.Max(Math.Min(Duration, start - Start), 0);
                long itemEnd = Start + Duration;
                long endOffset = Math.Max(Math.Min(Duration, itemEnd - end), 0);
                return Duration - startoffset - endOffset;
            }
            return 0;
        }

        public BoonsGraphModel.SegmentWithSources ToSegment()
        {
            return new BoonsGraphModel.SegmentWithSources(Start, End, GetStack(), GetSources().ToArray());
        }

        public abstract void SetEnd(long end);

        public abstract List<AgentItem> GetSources();

        public abstract int GetStack();
    }
}
