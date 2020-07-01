using System;

namespace GW2EIParser.EIData
{
    public class Segment
    {
        public long Start { get; set; }
        public long End { get; set; }
        public int Value { get; set; }

        public Segment(long start, long end, int value)
        {
            Start = start;
            End = end;
            Value = value;
        }

        public Segment(Segment other) : this(other.Start, other.End, other.Value)
        {

        }

        public bool Intersect(long start, long end)
        {
            long maxStart = Math.Max(start, Start);
            long minEnd = Math.Min(end, End);
            return minEnd - maxStart >= 0;
        }
    }
}
