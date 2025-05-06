namespace GW2EIEvtcParser.ParsedData;

public abstract class LogDateEvent : MetaDataEvent
{
    public readonly uint ServerUnixTimeStamp;
    public readonly uint LocalUnixTimeStamp;
    public readonly long Time;

    internal LogDateEvent(CombatItem evtcItem) : base(evtcItem)
    {
        Time = evtcItem.Time;
        ServerUnixTimeStamp = (uint)evtcItem.Value;
        LocalUnixTimeStamp = (uint)evtcItem.BuffDmg;
    }

}
