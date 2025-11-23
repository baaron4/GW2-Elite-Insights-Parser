using System.Diagnostics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal abstract class BuffSimulationItem : SimulationItem
{
    //NOTE(Rennorb): I changed the element to have start + end instead of start + duration to fix a bug. 
    // Apparently the original arc events have start + duration, so there might be value in returning it to the previous state.
    // Apart form the bug there doesn't seem to be a performance penalty either way,
    // however I did not investigate what penalty a fix for tat bug would incur (probably quite minor, something like one clamp).
    public readonly long Start;
    public long End;
    public long Duration  => End - Start;

    public BuffSimulationItem(long start, long end)
    {
        Start = start;
        End = end;
    }

    public long GetClampedDuration(long start, long end)
    {
        if (start >= end || start >= End || Start >= end)
        {
            return 0;
        }
        return Math.Max(0, Math.Clamp(End, start, end) - Math.Clamp(Start, start, end));
    }

    public long GetClampedDuration(long start, long end, SingleActor actor)
    {
        if (start >= end || start >= End || Start >= end)
        {
            return 0;
        }
        if (GetActiveStacks(actor) == 0)
        {
            return 0;
        }
        return Math.Max(0, Math.Clamp(End, start, end) - Math.Clamp(Start, start, end));
    }

    public Segment ToSegment()
    {
        return new Segment(Start, End, GetActiveStacks());
    }

    public Segment ToSegment(SingleActor actor)
    {
        return new Segment(Start, End, GetActiveStacks(actor));
    }

    /*public Segment ToDurationSegment()
    {
        return new Segment(Start, End, GetActualDuration());
    }*/

    public abstract void OverrideEnd(long end);
    //public abstract IEnumerable<long> GetActualDurationPerStack();
    //public abstract long GetActualDuration();

    public abstract IEnumerable<AgentItem> GetSources();
    public abstract IEnumerable<AgentItem> GetActiveSources();

    public abstract int GetActiveStacks();
    public abstract int GetStacks();
    public abstract int GetActiveStacks(SingleActor actor);
    public abstract int GetStacks(SingleActor actor);
}
