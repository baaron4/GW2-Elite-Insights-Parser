using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal abstract class BuffSimulationItem : AbstractSimulationItem
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

        public Segment ToSegment()
        {
            return new Segment(Start, End, GetActiveStacks());
        }

        public Segment ToSegment(AbstractSingleActor actor)
        {
            return new Segment(Start, End, GetActiveStacks(actor));
        }

        public Segment ToDurationSegment()
        {
            return new Segment(Start, End, GetActualDuration());
        }

        public abstract void OverrideEnd(long end);
        public abstract IReadOnlyList<long> GetActualDurationPerStack();
        public abstract long GetActualDuration();

        public abstract IReadOnlyList<AgentItem> GetSources();
        public abstract IReadOnlyList<AgentItem> GetActiveSources();
        public abstract IReadOnlyList<AgentItem> GetSources(AbstractSingleActor actor);
        public abstract IReadOnlyList<AgentItem> GetActiveSources(AbstractSingleActor actor);

        public abstract int GetActiveStacks();
        public abstract int GetStacks();
        public abstract int GetActiveStacks(AbstractSingleActor actor);
        public abstract int GetStacks(AbstractSingleActor actor);
    }
}
