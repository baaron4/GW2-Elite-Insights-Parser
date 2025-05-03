namespace GW2EIEvtcParser.ParsedData;

public class SkillGUIDEvent : IDToGUIDEvent
{
    internal SkillGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    { 
    }
}

