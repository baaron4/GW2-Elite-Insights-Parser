namespace GW2EIEvtcParser;

public abstract class AbstractCachingCollection<T>
{

    private readonly long _start;
    private readonly long _end;

    protected AbstractCachingCollection(ParsedEvtcLog log)
    {
        _start = log.LogData.EvtcLogStart;
        _end = log.LogData.EvtcLogEnd;
    }

    protected (long, long) SanitizeTimes(long start, long end)
    {
        long newStart = Math.Max(start, _start);
        long newEnd = Math.Max(newStart, Math.Min(end, _end));
        return (newStart, newEnd);
    }

    public abstract void Clear();

}
