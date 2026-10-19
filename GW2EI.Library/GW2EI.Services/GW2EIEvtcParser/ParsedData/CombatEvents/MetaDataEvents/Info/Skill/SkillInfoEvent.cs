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

    public (uint lower, uint upper) LandLoopSteps { get; protected set; }
    public (uint lower, uint upper) UnderwaterLoopSteps { get; protected set; }
    public IReadOnlyList<SkillTiming> SkillTimings => _SkillTimings;
    public IReadOnlyList<SkillStep> SkillSteps => _SkillSteps;

    private readonly List<SkillTiming> _SkillTimings = [];

    private readonly List<SkillStep> _SkillSteps = [];

    internal SkillInfoEvent(CombatItem evtcItem, SkillData skillData, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
        Skill = skillData.Get(evtcItem.SkillID);
        CompleteSkillInfoEvent(evtcItem, evtcVersion);
    }

    internal void CompleteSkillInfoEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion)
    {
        if (evtcItem.SkillID != SkillID)
        {
            throw new InvalidOperationException("Non matching buff id in SkillInfoEvent complete method");
        }
        switch (evtcItem.IsStateChange)
        {
            case StateChange.SkillTiming:
                BuildFromSkillTiming(evtcItem, evtcVersion);
                break;
            case StateChange.SkillInfo:
                BuildFromSkillInfo(evtcItem, evtcVersion);
                break;
            case StateChange.SkillStep:
                break;
            default:
                throw new InvalidDataException("Invalid combat event in SkillInfoEvent complete method");
        }
    }

    private unsafe void BuildFromSkillInfo(CombatItem evtcItem, EvtcVersionEvent evtcVersion)
    {
        // 2 
        var time = evtcItem.Time;
        // 2
        var srcAgent = evtcItem.SrcAgent;

        Recharge = *(float*)&time;
        Range0 = *((float*)&time + 1);
        Range1 = *(float*)&srcAgent;
        TooltipTime = *((float*)&srcAgent + 1);

        if (evtcVersion.Build >= ArcDPSBuilds.SkillInfoRework)
        {
            // 2
            var dstAgent = evtcItem.DstAgent;
            LandLoopSteps = (*(uint*)&dstAgent, *((uint*)&dstAgent + 1));

            UnderwaterLoopSteps = ((uint)evtcItem.BuffDmg, (uint)evtcItem.Value);
        }
    }

    private void BuildFromSkillTiming(CombatItem evtcItem, EvtcVersionEvent evtcVersion)
    {
        if (evtcVersion.Build >= ArcDPSBuilds.SkillInfoRework)
        {
            _SkillTimings.Add(new SkillTimingWithSteps(evtcItem, this));
        }
        else
        {
            _SkillTimings.Add(new SkillTimingNoSteps(evtcItem, this));
        }
    }

    private void BuildFromSkillStep(CombatItem evtcItem)
    {
        _SkillSteps.Add(new SkillStep(evtcItem, this));
    }

}
