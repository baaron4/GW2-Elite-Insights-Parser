using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GW2EIEvtcParser.ParsedData;

public abstract class AbstractTimeCombatEvent
{
    public long Time { get; protected set; }

    public AbstractTimeCombatEvent(long time)
    {
        Time = time;
    }
}

public static partial class ListExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SortByTime<T>(this List<T> list)  where T : AbstractTimeCombatEvent
    {
        StableSort<T>.fluxsort(list.AsSpan(), (a, b) => a.Time.CompareTo(b.Time));
    }

    /// Finds the event that is fist in the timeline and has a time above zero.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstByNonZeroTime<T>(this IReadOnlyList<T> events) where T : AbstractTimeCombatEvent
    {
        (T? Agent, long Time) result = (default, long.MaxValue);
        foreach(var @event in events)
        {
            if(@event.Time < result.Time)
            {
                result = (@event, @event.Time);
            }
        }
        return result.Agent;
    }
}
