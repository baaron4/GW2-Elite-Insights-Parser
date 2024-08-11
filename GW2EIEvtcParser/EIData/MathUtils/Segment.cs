using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class Segment : GenericSegment<double>
    {

        public Segment(long start, long end, double value) : base(start, end, value)
        {
        }

        public Segment(long start, long end) : this(start, end, 0)
        {
        }

        public Segment(Segment other) : this(other.Start, other.End, other.Value)
        {

        }

        public double IntersectingArea(Segment seg)
        {
            return IntersectingArea(seg.Start, seg.End);
        }

        public double IntersectingArea(long start, long end)
        {
            long maxStart = Math.Max(start, Start);
            long minEnd = Math.Min(end, End);
            return Math.Max(minEnd - maxStart, 0) * Value;
        }

        public static List<Segment> FromStates(List<(long start, double state)> states, long min, long max)
        {
            if (states.Count == 0)
            {
                return new List<Segment>();
            }
            var res = new List<Segment>();
            double lastValue = states.First().state;
            foreach ((long start, double state) in states)
            {
                long end = Math.Min(Math.Max(start, min), max);
                if (res.Count == 0)
                {
                    res.Add(new Segment(0, end, lastValue));
                }
                else
                {
                    res.Add(new Segment(res.Last().End, end, lastValue));
                }
                lastValue = state;
            }
            res.Add(new Segment(res.Last().End, max, lastValue));
            res.RemoveAll(x => x.Start >= x.End);
            return FuseSegments(res);
        }


        /// <summary>
        /// Fuse consecutive segments with same value
        /// </summary>
        public static List<Segment> FuseSegments(List<Segment> input)
        {
            var newChart = new List<Segment>();
            Segment last = null;
            foreach (Segment seg in input)
            {
                if (seg.Start == seg.End)
                {
                    continue;
                }
                if (last == null)
                {
                    newChart.Add(new Segment(seg));
                    last = newChart.Last();
                }
                else
                {
                    if (seg.Value == last.Value)
                    {
                        last.End = seg.End;
                    }
                    else
                    {
                        newChart.Add(new Segment(seg));
                        last = newChart.Last();
                    }
                }
            }
            return newChart;
        }

        public static List<object[]> ToObjectList(List<Segment> segments, long start, long end)
        {
            var res = new List<object[]>();
            foreach (Segment seg in segments)
            {
                double segStart = Math.Round(Math.Max(seg.Start - start, 0) / 1000.0, ParserHelper.TimeDigit);
                res.Add(new object[] { segStart, seg.Value });
            }
            Segment lastSeg = segments.Last();
            double segEnd = Math.Round(Math.Min(lastSeg.End - start, end - start) / 1000.0, ParserHelper.TimeDigit);
            res.Add(new object[] { segEnd, lastSeg.Value });
            return res;
        }
        // https://www.c-sharpcorner.com/blogs/binary-search-implementation-using-c-sharp1
        public static int BinarySearchRecursive(IReadOnlyList<Segment> segments, long time, int minIndex, int maxIndex)
        {
            if (segments[minIndex].Start > time)
            {
                return minIndex;
            }
            if (segments[maxIndex].Start < time)
            {
                return maxIndex;
            }
            if (minIndex > maxIndex)
            {
                return minIndex;
            }
            else
            {
                int midIndex = (minIndex + maxIndex) / 2;
                if (segments[midIndex].ContainsPoint(time))
                {
                    return midIndex;
                }
                else if (time < segments[midIndex].Start)
                {
                    return BinarySearchRecursive(segments, time, minIndex, midIndex - 1);
                }
                else
                {
                    return BinarySearchRecursive(segments, time, midIndex + 1, maxIndex);
                }
            }
        }
    }
}
