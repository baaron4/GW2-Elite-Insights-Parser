using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BoonsGraphModel
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

            public Segment(Segment other)
            {
                Start = other.Start;
                End = other.End;
                Value = other.Value;
            }
        }

        public readonly Boon Boon;
        public List<Segment> BoonChart { get; private set; } = new List<Segment>();

        // Constructor
        public BoonsGraphModel(Boon boon)
        {
            Boon = boon;
        }
        public BoonsGraphModel(Boon boon, List<Segment> boonChart)
        {
            Boon = boon;
            BoonChart = boonChart;
            FuseSegments();
        }

        public int GetStackCount(long time)
        {
            foreach (Segment seg in BoonChart)
            {
                if (seg.Start <= time && time <= seg.End)
                {
                    return seg.Value;
                }
            }
            return 0;
        }

        public void FuseSegments()
        {
            List<Segment> newChart = new List<Segment>();
            Segment last = null;
            foreach (Segment seg in BoonChart)
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
            BoonChart = newChart;
        }

    }
}
