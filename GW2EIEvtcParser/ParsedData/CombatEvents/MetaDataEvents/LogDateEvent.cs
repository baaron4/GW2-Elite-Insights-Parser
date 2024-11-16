namespace GW2EIEvtcParser.ParsedData;

public abstract class LogDateEvent : AbstractMetaDataEvent
{
    public readonly uint ServerUnixTimeStamp;
    public readonly uint LocalUnixTimeStamp;

    internal LogDateEvent(CombatItem evtcItem) : base(evtcItem)
    {
        ServerUnixTimeStamp = (uint)evtcItem.Value;
        LocalUnixTimeStamp = (uint)evtcItem.BuffDmg;
    }

}
