using System;
using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class BuffSimulationItem : AbstractBuffSimulationItem
    {
        public long Duration { get; protected set; }
        public long Start { get; protected set; }
        public long End => Start + Duration;

        protected BuffSimulationItem(long start, long duration)
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

        public BuffsGraphModel.SegmentWithSources ToSegment()
        {
            return new BuffsGraphModel.SegmentWithSources(Start, End, GetStack(), GetSources().ToArray());
        }

        public abstract void OverrideEnd(long end);

        public abstract List<AgentItem> GetSources();

        public abstract int GetStack();
    }
}
