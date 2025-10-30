namespace GW2EIEvtcParser.EIData;

public static class SegmentExt {
    public static double IntersectingArea(this in Segment self, in Segment other) => IntersectingArea(self, other.Start, other.End);

    public static double IntersectingArea(this in Segment self, long start, long end)
    {
        long maxStart = Math.Max(start, self.Start);
        long minEnd = Math.Min(end, self.End);
        return Math.Max(minEnd - maxStart, 0) * self.Value;
    }

    
    /// <summary>
    /// Fuse consecutive segments with same value. The list should not be empty.
    /// </summary>
    public static void FuseConsecutive<T>(this List<GenericSegment<T>> segments)
    {

        GenericSegment<T> last = segments[0];
        int lastIndex = 0;
        for(int i = 1; i < segments.Count; i++)
        {
            var current = segments[i];
            //TODO_PERF(Rennorb)
            if (current.IsEmpty())
            {
                continue;
            }

            if (current.Value != null && current.Value.Equals(last.Value))
            {
                last.End = current.End;
                segments[lastIndex] = last;
            }
            else
            {
                segments[lastIndex] = last;
                last = current;
                lastIndex++;
            }
        }
        segments[lastIndex++] = last;

        segments.RemoveRange(lastIndex, segments.Count - lastIndex);
    }

    //TODO(Rennorb) @cleanup @perf
    public static List<double[]> ToObjectList(this IReadOnlyList<Segment> segments, long start, long end)
    {
        var res = new List<double[]>(segments.Count + 1);
        foreach (Segment seg in segments)
        {
            double segStart = Math.Round(Math.Max(seg.Start - start, 0) / 1000.0, ParserHelper.TimeDigit);
            res.Add([segStart, seg.Value]);
        }
        Segment lastSeg = segments.Last();
        double segEnd = Math.Round(Math.Min(lastSeg.End - start, end - start) / 1000.0, ParserHelper.TimeDigit);
        res.Add([segEnd, lastSeg.Value]);
        return res;
    }

    //TODO(Rennorb) @cleanup
    // https://www.c-sharpcorner.com/blogs/binary-search-implementation-using-c-sharp1
    public static int BinarySearchRecursive(this IReadOnlyList<Segment> segments, long time, int minIndex, int maxIndex)
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
