namespace GW2EIEvtcParser.ParsedData;

public class SkillTimingNoSteps : SkillTiming
{
    internal SkillTimingNoSteps(CombatItem evtcItem, SkillAnimation animation) : base(evtcItem, animation)
    {
        Time = (int)evtcItem.DstAgent;
    }
}
