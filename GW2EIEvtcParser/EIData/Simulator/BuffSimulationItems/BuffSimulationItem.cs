using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class BuffSimulationItem : AbstractSimulationItem
    {
        public long Duration { get; protected set; }
        protected long OriginalDuration { get; }
        public long Start { get; protected set; }
        public long End => Start + Duration;

        protected BuffSimulationItem(long start, long duration)
        {
            Start = start;
            Duration = duration;
            OriginalDuration = duration;
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

        public Segment ToSegment()
        {
            return new Segment(Start, End, GetActiveStacks());
        }

        public abstract void OverrideEnd(long end);
        public abstract IReadOnlyList<long> GetActualDurationPerStack();

        public abstract List<AgentItem> GetSources();

        public abstract int GetActiveStacks();
        public abstract int GetStacks();
    }
}
