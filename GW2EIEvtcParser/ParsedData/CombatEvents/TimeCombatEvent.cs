using System.Runtime.CompilerServices;

namespace GW2EIEvtcParser.ParsedData;

public abstract class TimeCombatEvent
{
    public long Time { get; protected set; }

    protected TimeCombatEvent(long time)
    {
        Time = time;
    }
}

public static partial class ListExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SortByTime<T>(this List<T> list)  where T : TimeCombatEvent
    {
        list.AsSpan().SortStable((a, b) => a.Time.CompareTo(b.Time));
    }
}
