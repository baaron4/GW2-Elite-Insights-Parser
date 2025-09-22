namespace GW2EIEvtcParser.ParsedData;

public class SquadCombatEndEvent : LogDateEvent
{
    public readonly bool ByPoVMapExit = false;
    internal SquadCombatEndEvent(CombatItem evtcItem) : base(evtcItem)
    {
        ByPoVMapExit = (evtcItem.DstAgent & 0x00000001) == 1;
    }

}
