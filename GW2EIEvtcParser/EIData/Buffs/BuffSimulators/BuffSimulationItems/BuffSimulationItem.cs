using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    /// <summary> A segment of time with type <see cref="double"/> with inclusive start and inclusive end. </summary>
    using Segment = GenericSegment<double>;

    internal abstract class BuffSimulationItem : AbstractSimulationItem
    {
        public long Duration { get; protected set; }
        public readonly long Start;
        public long End => Start + Duration;

        protected BuffSimulationItem(long start, long duration)
        {
            Start = start;
            Duration = duration;
        }

        public long GetClampedDuration(long start, long end)
        {
            return Math.Max(0, Math.Clamp(this.End, start, end) - Math.Clamp(this.Start, start, end));
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
        public abstract IEnumerable<long> GetActualDurationPerStack();
        public abstract long GetActualDuration();

        public abstract IEnumerable<AgentItem> GetSources();
        public abstract IEnumerable<AgentItem> GetActiveSources();

        public abstract int GetActiveStacks();
        public abstract int GetStacks();
        public abstract int GetActiveStacks(AbstractSingleActor actor);
        public abstract int GetStacks(AbstractSingleActor actor);
    }
}
