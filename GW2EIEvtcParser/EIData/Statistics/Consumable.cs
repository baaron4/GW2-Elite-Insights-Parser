namespace GW2EIEvtcParser.EIData;

public class Consumable
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
}
