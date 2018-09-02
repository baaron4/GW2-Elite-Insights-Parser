using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BoonsGraphModel
    {

        public class Segment
        {
            public readonly long Start;
            public long End { get; set; }
            public readonly int Value;

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

        private readonly string _boonName;
        private readonly List<Segment> _boonChart = new List<Segment>();

        // Constructor
        public BoonsGraphModel(string boonName)
        {
            _boonName = boonName;
        }
        public BoonsGraphModel(string boonName, List<Segment> boonChart)
        {
            _boonName = boonName;
            // Fuse segments
            Segment last = null;
            foreach(Segment seg in boonChart)
            {
                if (last == null)
                {
                    _boonChart.Add(new Segment(seg));
                    last = _boonChart.Last();
                } else
                {
                    if (seg.Value == last.Value)
                    {
                        last.End = seg.End;
                    } else
                    {
                        _boonChart.Add(new Segment(seg));
                        last = _boonChart.Last();
                    }
                }
            }
        }
        //getters
        public string GetBoonName() {
            return _boonName;
        }
        public List<Segment> GetBoonChart()
        {
            return _boonChart;
        }

    }
}
