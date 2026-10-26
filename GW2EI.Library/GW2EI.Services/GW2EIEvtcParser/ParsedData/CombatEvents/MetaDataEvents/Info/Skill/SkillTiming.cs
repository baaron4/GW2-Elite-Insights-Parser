namespace GW2EIEvtcParser.ParsedData;

using static GW2EIEvtcParser.ArcDPSEnums;

public abstract class SkillTiming
{
    internal readonly SkillAnimation Animation;
    public readonly byte TriggerByte;
    public readonly SkillTrigger Trigger;
    public int Time { get; protected set; }

    public bool IsControlReturn => Trigger == SkillTrigger.ControlReturn;
    public bool IsAnyExecute => IsCastBarExecute || Trigger == SkillTrigger.Execute4 || Trigger == SkillTrigger.Execute5;
    public bool IsCastBarExecute => Trigger == SkillTrigger.Execute3;

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
            return new SkillTimingWithFlags(evtcItem, animation);
        }
        else
        {
            return new SkillTimingNoFlags(evtcItem, animation);
        }
    }

    private static bool SupportsSteps(CombatItem evtcItem)
    {
        return evtcItem.Build >= ArcDPSBuilds.SkillInfoRework;
    }

    internal static bool IsUnderwater(CombatItem evtcItem)
    {
        return SupportsSteps(evtcItem) && SkillTimingWithFlags.IsUnderwater(evtcItem);
    }
}
