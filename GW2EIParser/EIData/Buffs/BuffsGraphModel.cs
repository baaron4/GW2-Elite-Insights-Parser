using System.Collections.Generic;
using System.Linq;

namespace GW2EIParser.EIData
{
    public class BuffsGraphModel
    {
        public Buff Buff { get; }
        public List<BuffSegment> BuffChart { get; private set; } = new List<BuffSegment>();

        // Constructor
        public BuffsGraphModel(Buff buff)
        {
            Buff = buff;
        }
        public BuffsGraphModel(Buff buff, List<BuffSegment> buffChartWithSource)
        {
            Buff = buff;
            BuffChart = buffChartWithSource;
            FuseSegments();
        }

        public int GetStackCount(long time)
        {
            for (int i = BuffChart.Count - 1; i >= 0; i--)
            {
                BuffSegment seg = BuffChart[i];
                if (seg.Start <= time && time <= seg.End)
                {
                    return seg.Value;
                }
            }
            return 0;
        }


        public bool IsPresent(long time, long window)
        {
            int count = 0;
            foreach (BuffSegment seg in BuffChart)
            {
                if (seg.Intersect(time - window, time + window))
                {
                    count += seg.Value;
                }
            }
            return count > 0;
        }

        public void FuseSegments()
        {
            var newChart = new List<BuffSegment>();
            BuffSegment last = null;
            foreach (BuffSegment seg in BuffChart)
            {
                if (seg.Start == seg.End)
                {
                    continue;
                }
                if (last == null)
                {
                    newChart.Add(new BuffSegment(seg));
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
                        newChart.Add(new BuffSegment(seg));
                        last = newChart.Last();
                    }
                }
            }
            BuffChart = newChart;
        }

    }
}
