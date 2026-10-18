namespace GW2EIEvtcParser.ParsedData;

using static GW2EIEvtcParser.ArcDPSEnums;

public abstract class SkillTiming
{
    internal readonly SkillInfoEvent SkillInfo;
    public readonly ulong TriggerUlong;

    public readonly TriggerType Trigger;

    internal SkillTiming(CombatItem evtcItem, SkillInfoEvent skillInfo)
    {
        SkillInfo = skillInfo;
        TriggerUlong = evtcItem.SrcAgent;
        Trigger = GetSkillAction(TriggerUlong);
    }

}
