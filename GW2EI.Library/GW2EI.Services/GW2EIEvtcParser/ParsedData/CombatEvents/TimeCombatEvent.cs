using System.Runtime.CompilerServices;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Interfaces;

namespace GW2EIEvtcParser.ParsedData;

public abstract class TimeCombatEvent : CombatEvent, ITimeComparable
{
    public long Time { get; protected set; }

    protected TimeCombatEvent(long time)
    {
        Time = time;
    }

    public bool IsAtTime(long time, long epsilon = ParserHelper.ServerDelayConstant)
    {
        return Math.Abs(time - Time) < epsilon;
    }
    public bool IsAtTime(TimeCombatEvent evt, long epsilon = ParserHelper.ServerDelayConstant)
    {
        return IsAtTime(evt.Time, epsilon);
    }
    public bool IsAtTime(CombatItem evt, long epsilon = ParserHelper.ServerDelayConstant)
    {
        return IsAtTime(evt.Time, epsilon);
    }
}

public static partial class ListExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SortByTime<T>(this List<T> list) where T : TimeCombatEvent
    {
        list.AsSpan().SortStable((a, b) => a.Time.CompareTo(b.Time));
    }
}
