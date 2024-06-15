using System;

namespace GW2EIEvtcParser.EIData
{
    public class GenericSegment<T>
    {
        public long Start { get; internal set; }
        public long End { get; internal set; }
        public T Value { get; internal set; }

        public GenericSegment(long start, long end, T value)
        {
            Start = start;
            End = end;
            Value = value;
        }

        public GenericSegment(long start, long end) : this(start, end, default)
        {
        }

        public GenericSegment(GenericSegment<T> other) : this(other.Start, other.End, other.Value)
        {

        }

        public bool IntersectSegment(GenericSegment<T> seg)
        {
            return IntersectSegment(seg.Start, seg.End);
        }

        public bool IntersectSegment(long start, long end)
        {
            if (Start > End)
            {
                return false;
            }
            else if (Start == End)
            {
                return ContainsPoint(start);
            }
            long maxStart = Math.Max(start, Start);
            long minEnd = Math.Min(end, End);
            return minEnd - maxStart >= 0;
        }

        public bool ContainsPoint(long time)
        {
            return Start <= time && End >= time;
        }
    }
}
