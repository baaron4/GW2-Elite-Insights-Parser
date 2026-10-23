namespace GW2EIEvtcParser.ParsedData;

using static GW2EIEvtcParser.ArcDPSEnums;

public abstract class SkillTiming
{
    internal readonly SkillAnimation Animation;
    public readonly byte TriggerByte;
    public readonly SkillTrigger Trigger;
    public int Time { get; protected set; }

    public bool IsControlReturn => Trigger == SkillTrigger.ControlReturn;
    public bool IsExecute => Trigger == SkillTrigger.Execute3 || Trigger == SkillTrigger.Execute4 || Trigger == SkillTrigger.Execute5;

    protected SkillTiming(CombatItem evtcItem, SkillAnimation animation)
    {
        Animation = animation;
        TriggerByte = (byte)evtcItem.SrcAgent;
        Trigger = GetSkillTrigger(TriggerByte);
    }


    internal static SkillTiming Build(CombatItem evtcItem, SkillAnimation animation)
    {
        if (SupportsSteps(evtcItem))
        {
            return new SkillTimingWithSteps(evtcItem, animation);
        }
        else
        {
            return new SkillTimingNoSteps(evtcItem, animation);
        }
    }

    public static bool SupportsSteps(CombatItem evtcItem)
    {
        return evtcItem.Build >= ArcDPSBuilds.SkillInfoRework;
    }

    public static bool IsUnderwater(CombatItem evtcItem)
    {
        return SupportsSteps(evtcItem) ? SkillTimingWithSteps.IsUnderwater(evtcItem) : false;
    }
}
