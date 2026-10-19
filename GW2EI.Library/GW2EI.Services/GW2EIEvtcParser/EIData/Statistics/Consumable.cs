using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class Consumable: ITimeComparable
{
    public readonly Buff Buff;
    public readonly long Time;
    public readonly int Duration;
    public int Stack { get; internal set; }

    public Consumable(Buff item, long time, int duration)
    {
        Buff = item;
        Time = time;
        Duration = duration;
        Stack = 1;
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
