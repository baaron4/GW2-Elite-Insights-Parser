namespace GW2EIEvtcParser.ParsedData;

public class SquadCombatStartEvent : LogDateEvent
{
    public readonly ArcDPSEnums.LogType LogType;
    internal SquadCombatStartEvent(CombatItem evtcItem) : base(evtcItem)
    {
        LogType = GetLogType(evtcItem);
    }
    internal static ArcDPSEnums.LogType GetLogType(CombatItem evtcItem)
    {
        return ArcDPSEnums.GetLogType((int)evtcItem.DstAgent);
    }
}
