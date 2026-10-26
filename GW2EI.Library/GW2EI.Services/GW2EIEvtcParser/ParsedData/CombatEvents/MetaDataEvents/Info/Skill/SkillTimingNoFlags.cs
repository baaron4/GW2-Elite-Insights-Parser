namespace GW2EIEvtcParser.ParsedData;

public class SkillTimingNoFlags : SkillTiming
{
    internal SkillTimingNoFlags(CombatItem evtcItem, SkillAnimation animation) : base(evtcItem, animation)
    {
        Time = (int)evtcItem.DstAgent;
    }
}
