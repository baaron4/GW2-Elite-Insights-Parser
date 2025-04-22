namespace GW2EIEvtcParser.EIData;

public class BuffGraph
{
    public readonly Buff Buff;

    public IReadOnlyList<Segment> Values => _buffChart.Values;
    private StateGraph<double> _buffChart;
    /// <summary>
    /// Does not contain any > 0 values
    /// </summary>
    public bool IsEmpty { get; private set; }
    /// <summary>
    /// Contains only > 0 values
    /// </summary>
    public bool IsFull { get; private set; }

    // Constructor
    internal BuffGraph(Buff buff)
    {
        Buff = buff;
        _buffChart = new StateGraph<double>();
        IsEmpty = true;
        IsFull = false;
    }

    internal BuffGraph(Buff buff, List<Segment> buffChartWithSource)
    {
        Buff = buff;
        _buffChart = new StateGraph<double>(buffChartWithSource);
        _buffChart.FuseSegments();
        IsEmpty = !_buffChart.Values.Any(x => x.Value > 0);
        IsFull = _buffChart.Values.All(x => x.Value > 0);
    }

    public Segment GetBuffStatus(long time)
    {
        if (Values.Count == 0)
        {
            return new Segment(long.MinValue, long.MaxValue, 0);
        }
        
        int foundIndex = Values.BinarySearchRecursive(time, 0, Values.Count - 1);
        Segment found = Values[foundIndex];
        if (found.ContainsPoint(time))
        {
            return found;
        }

        return new Segment(long.MinValue, long.MaxValue, 0);
    }

    public IEnumerable<Segment> GetBuffStatus(long start, long end)
    {
        return Values.Where(seg => seg.Intersects(start, end));
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
    /// This method will integrate the graph "from" to "to"
    /// It is going to add +1 to "to" when "from" has a value > 0
    /// </summary>
    internal void MergePresenceInto(IReadOnlyList<Segment> from)
    {
        if (_buffChart.Values.Count == 0)
        {
            _buffChart = new StateGraph<double>(from.Select(x => new Segment(x.Start, x.End, x.Value > 0 ? 1 : 0)));
        }
        else
        {
            //TODO(Rennorb) @perf
            var segmentsToFill = new LinkedList<Segment>(_buffChart.Values);
            var node = segmentsToFill.First;
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

                    curSeg.End = start;
                    node.Value = curSeg;

                    // The segment in inside current one
                    if (end <= curEnd)
                    {
                        segmentsToFill.AddAfter(node, new Segment(start, end, curVal + presence));
                        node = node.Next;
                        if (node != null)
                        {
                            segmentsToFill.AddAfter(node, new Segment(end, curEnd, curVal));
                            node = node.Next;
                        }
                        break;
                    }
                    else
                    {
                        // the segment straddles cur and next
                        segmentsToFill.AddAfter(node, new Segment(start, curEnd, curVal + presence));
                        node = node.Next;
                        start = curEnd;
                    }
                    if (node != null)
                    {
                        node = node.Next;
                    }
                }
            }
            _buffChart = new StateGraph<double>(segmentsToFill.ToList());
        }
        // Merge consecutive segments with same value, otherwise expect exponential growth
        _buffChart.FuseSegments();
        // If the graph was not empty, this method can not empty it
        if (!IsEmpty)
        {
            IsEmpty = !_buffChart.Values.Any(x => x.Value > 0);
        }
        // This method can not add 0s to the graph
        if (IsFull)
        {
            IsFull = _buffChart.Values.All(x => x.Value > 0);
        }
    }

}
