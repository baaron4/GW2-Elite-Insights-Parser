using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIUtils;

namespace GW2EIParser.EIData
{
    public class Segment
    {
        public long Start { get; set; }
        public long End { get; set; }
        public double Value { get; set; }

        public Segment(long start, long end, double value)
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

        public static List<Segment> FromStates(List<(long start, double state)> states, long min, long max)
        {
            if (!states.Any())
            {
                return new List<Segment>();
            }
            var res = new List<Segment>();
            double lastValue = states.First().state;
            foreach ((long start, double state) in states)
            {
                long end = Math.Min(Math.Max(start, min), max);
                if (!res.Any())
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
            res.RemoveAll(x => x.Start == x.End);
            return res;
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
                double segStart = Math.Round(Math.Max(seg.Start - start, 0) / 1000.0, GeneralHelper.TimeDigit);
                res.Add(new object[] { segStart, seg.Value });
            }
            Segment lastSeg = segments.Last();
            double segEnd = Math.Round(Math.Min(lastSeg.End - start, end - start) / 1000.0, GeneralHelper.TimeDigit);
            res.Add(new object[] { segEnd, lastSeg.Value });
            return res;
        }
    }
}
