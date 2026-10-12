namespace GW2EIEvtcParser.ParsedData;

public class PvPScoreUpdateEvent : TimeCombatEvent
{
    public readonly uint Score;
    public readonly uint TeamID;

    public readonly uint Content332ID; // what is this?

    internal PvPScoreUpdateEvent(CombatItem evtcItem) : base(evtcItem.Time)
    {
        Score = (uint)evtcItem.Value;
        TeamID = (uint)evtcItem.BuffDmg;
        Content332ID = evtcItem.SkillID;
    }

}
