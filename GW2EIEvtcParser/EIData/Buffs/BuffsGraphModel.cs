using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GW2EIEvtcParser.EIData.BuffSimulators;

namespace GW2EIEvtcParser.EIData
{
    public class BuffsGraphModel
    {
        public Buff Buff { get; }

        public IReadOnlyList<Segment> BuffChart => _buffChart;
        private List<Segment> _buffChart { get; set; } = new List<Segment>();

        // Constructor
        internal BuffsGraphModel(Buff buff)
        {
            Buff = buff;
        }
        internal BuffsGraphModel(Buff buff, List<Segment> buffChartWithSource)
        {
            Buff = buff;
            _buffChart = buffChartWithSource;
            FuseSegments();
        }

        public Segment GetBuffStatus(long time)
        {
            foreach (Segment seg in BuffChart)
            {
                if (seg.ContainsPoint(time))
                {
                    return seg;
                }
            }
            return new Segment(long.MinValue, long.MaxValue, 0);
        }

        public IReadOnlyList<Segment> GetBuffStatus(long start, long end)
        {
            var res = new List<Segment>();
            foreach (Segment seg in BuffChart)
            {
                if (seg.IntersectSegment(start, end))
                {
                    res.Add(seg);
                }
            }
            return res;
        }

        public int GetStackCount(long time)
        {
            return (int)GetBuffStatus(time).Value;
        }


        public bool IsPresent(long time, long window = 0)
        {
            if (window != 0)
            {
                long absWindow = Math.Abs(window);
                return GetBuffStatus(time - absWindow, time + absWindow).Any(x => x.Value > 0);
            }
            return GetStackCount(time) > 0;
        }

        /// <summary>
        /// Fuse consecutive segments with same value
        /// </summary>
        internal void FuseSegments()
        {
            _buffChart = Segment.FuseSegments(_buffChart);
            _buffChart.RemoveAll(x => x.Start > x.End);
        }

        /// <summary>
        /// This method will integrate the graph "from" to "to"
        /// It is going to add +1 to "to" when "from" has a value > 0
        /// </summary>
        /// <param name="from"></param> 
        /// <param name="to"></param>
        internal void MergePresenceInto(IReadOnlyList<Segment> from)
        {
            if (_buffChart.Count == 0)
            {
                _buffChart = new List<Segment>(from.Select(x => new Segment(x.Start, x.End, x.Value > 0 ? 1 : 0)));
            }
            else
            {
                var segmentsToFill = new LinkedList<Segment>(_buffChart);
                LinkedListNode<Segment> node = segmentsToFill.Find(segmentsToFill.First());
                foreach (Segment seg in from)
                {
                    long start = seg.Start;
                    long end = seg.End;
                    int presence = seg.Value > 0 ? 1 : 0;
                    // No need to process this segment
                    if (presence == 0)
                    {
                        continue;
                    }
                    while (node != null)
                    {
                        Segment curSeg = node.Value;
                        long curEnd = curSeg.End;
                        long curStart = curSeg.Start;
                        int curVal = (int)curSeg.Value;
                        if (curStart > end)
                        {
                            break;
                        }
                        if (curEnd < start)
                        {
                            node = node.Next;
                            continue;
                        }
                        // The segment in inside current one
                        if (end <= curEnd)
                        {
                            curSeg.End = start;
                            segmentsToFill.AddAfter(node, new Segment(start, end, curVal + presence));
                            node = node.Next;
                            segmentsToFill.AddAfter(node, new Segment(end, curEnd, curVal));
                            node = node.Next;
                            break;
                        }
                        else
                        {
                            // the segment straddles cur and next
                            curSeg.End = start;
                            segmentsToFill.AddAfter(node, new Segment(start, curEnd, curVal + presence));
                            node = node.Next;
                            start = curEnd;
                        }
                        node = node.Next;
                    }
                }
                _buffChart = segmentsToFill.ToList();
            }
            // Merge consecutive segments with same value, otherwise expect exponential growth
            FuseSegments();
        }

    }
}
