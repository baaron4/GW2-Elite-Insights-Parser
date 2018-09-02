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

        public readonly string BoonName;
        public List<Segment> BoonChart { get; private set; } = new List<Segment>();

        // Constructor
        public BoonsGraphModel(string boonName)
        {
            BoonName = boonName;
        }
        public BoonsGraphModel(string boonName, List<Segment> boonChart)
        {
            BoonName = boonName;
            BoonChart = boonChart;
            FuseChart();
        }

        public void FuseChart()
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
