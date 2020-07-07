using System.Collections.Generic;
using System.Linq;

namespace GW2EIParser.EIData
{
    public class BuffsGraphModel
    {
        public Buff Buff { get; }
        public List<Segment> BuffChart { get; private set; } = new List<Segment>();

        // Constructor
        public BuffsGraphModel(Buff buff)
        {
            Buff = buff;
        }
        public BuffsGraphModel(Buff buff, List<Segment> buffChartWithSource)
        {
            Buff = buff;
            BuffChart = buffChartWithSource;
            FuseSegments();
        }

        public int GetStackCount(long time)
        {
            for (int i = BuffChart.Count - 1; i >= 0; i--)
            {
                Segment seg = BuffChart[i];
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
            foreach (Segment seg in BuffChart)
            {
                if (seg.Intersect(time - window, time + window))
                {
                    count += seg.Value;
                }
            }
            return count > 0;
        }

        /// <summary>
        /// Fuse consecutive segments with same value
        /// </summary>
        public void FuseSegments()
        {
            var newChart = new List<Segment>();
            Segment last = null;
            foreach (Segment seg in BuffChart)
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
            BuffChart = newChart;
        }

        /// <summary>
        /// This method will integrate the graph "from" to "to"
        /// It is going to add +1 to "to" when "from" has a value > 0
        /// </summary>
        /// <param name="from"></param> 
        /// <param name="to"></param>
        public void MergePresenceInto(List<Segment> from)
        {
            List<Segment> segmentsToFill = BuffChart;
            bool firstPass = segmentsToFill.Count == 0;
            foreach (Segment seg in from)
            {
                long start = seg.Start;
                long end = seg.End;
                int presence = seg.Value > 0 ? 1 : 0;
                if (firstPass)
                {
                    segmentsToFill.Add(new Segment(start, end, presence));
                }
                else
                {
                    for (int i = 0; i < segmentsToFill.Count; i++)
                    {
                        Segment curSeg = segmentsToFill[i];
                        long curEnd = curSeg.End;
                        long curStart = curSeg.Start;
                        int curVal = curSeg.Value;
                        if (curStart > end)
                        {
                            break;
                        }
                        if (curEnd < start)
                        {
                            continue;
                        }
                        if (end <= curEnd)
                        {
                            curSeg.End = start;
                            segmentsToFill.Insert(i + 1, new Segment(start, end, curVal + presence));
                            segmentsToFill.Insert(i + 2, new Segment(end, curEnd, curVal));
                            break;
                        }
                        else
                        {
                            curSeg.End = start;
                            segmentsToFill.Insert(i + 1, new Segment(start, curEnd, curVal + presence));
                            start = curEnd;
                            i++;
                        }
                    }
                }
            }
            // Merge consecutive segments with same value, otherwise expect exponential growth
            FuseSegments();
        }

    }
}
