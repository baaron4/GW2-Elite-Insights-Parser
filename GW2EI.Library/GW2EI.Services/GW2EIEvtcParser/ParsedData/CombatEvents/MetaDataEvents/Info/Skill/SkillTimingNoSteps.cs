namespace GW2EIEvtcParser.ParsedData;

public class SkillTimingNoSteps : SkillTiming
{
    public readonly ulong AtMillisecond;

    internal SkillTimingNoSteps(CombatItem evtcItem, SkillInfoEvent skillInfo) : base(evtcItem, skillInfo)
    {
        AtMillisecond = evtcItem.DstAgent;
    }

}
