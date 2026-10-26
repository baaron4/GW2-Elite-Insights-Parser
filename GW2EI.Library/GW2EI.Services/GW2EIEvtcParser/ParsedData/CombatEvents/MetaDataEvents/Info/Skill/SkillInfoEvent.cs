namespace GW2EIEvtcParser.ParsedData;

using static GW2EIEvtcParser.ArcDPSEnums;

public class SkillInfoEvent : MetaDataEvent
{
    public readonly SkillItem Skill;
    public long SkillID => Skill.ID;

    public float Recharge { get; protected set; }

    public float Range0 { get; protected set; }

    public float Range1 { get; protected set; }

    public float TooltipTime { get; protected set; }

    public SkillAnimation LandAnimation { get; protected set; }
    public SkillAnimation UnderwaterAnimation { get; protected set; }

    internal SkillInfoEvent(CombatItem evtcItem, SkillData skillData) : base(evtcItem)
    {
        Skill = skillData.Get(evtcItem.SkillID);
        LandAnimation = new SkillAnimation(this);
        UnderwaterAnimation = new SkillAnimation(this);
        CompleteSkillInfoEvent(evtcItem);
        Skill.AttachSkillInfoEvent(this);
    }

    internal void CompleteSkillInfoEvent(CombatItem evtcItem)
    {
        if (evtcItem.SkillID != SkillID)
        {
            throw new InvalidOperationException("Non matching buff id in SkillInfoEvent complete method");
        }
        switch (evtcItem.IsStateChange)
        {
            case StateChange.SkillTiming:
                BuildFromSkillTiming(evtcItem);
                break;
            case StateChange.SkillInfo:
                BuildFromSkillInfo(evtcItem);
                break;
            case StateChange.SkillStep:
                BuildFromSkillStep(evtcItem);
                break;
            default:
                throw new InvalidDataException("Invalid combat event in SkillInfoEvent complete method");
        }
    }

    private unsafe void BuildFromSkillInfo(CombatItem evtcItem)
    {
        // 2
        var time = evtcItem.Time;
        // 2
        var srcAgent = evtcItem.SrcAgent;

        Recharge = *(float*)&time;
        Range0 = *((float*)&time + 1);
        Range1 = *(float*)&srcAgent;
        TooltipTime = *((float*)&srcAgent + 1);

        if (evtcItem.Build >= ArcDPSBuilds.SkillInfoRework)
        {
            // 2
            var dstAgent = evtcItem.DstAgent;
            LandAnimation.SetLoopSteps(*(uint*)&dstAgent, *((uint*)&dstAgent + 1));
            UnderwaterAnimation.SetLoopSteps((uint)evtcItem.BuffDmg, (uint)evtcItem.Value);
        }
    }

    private void BuildFromSkillTiming(CombatItem evtcItem)
    {
        GetAnimation(SkillTiming.IsUnderwater(evtcItem)).AddTiming(evtcItem);
    }

    private void BuildFromSkillStep(CombatItem evtcItem)
    {
        GetAnimation(SkillStep.IsUnderwater(evtcItem)).AddStep(evtcItem);
    }

    public SkillAnimation GetAnimation(bool water)
    {
        return water ? UnderwaterAnimation : LandAnimation;
    }
}
