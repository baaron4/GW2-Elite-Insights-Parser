namespace GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

public class SkillInfoEvent : MetaDataEvent
{
    public readonly uint SkillID;

    public float Recharge { get; protected set; }

    public float Range0 { get; protected set; }

    public float Range1 { get; protected set; }

    public float TooltipTime { get; protected set; }
    public IReadOnlyList<SkillTiming> SkillTimings => _SkillTimings;

    private readonly List<SkillTiming> _SkillTimings = [];

    internal SkillInfoEvent(CombatItem evtcItem) : base(evtcItem)
    {
        SkillID = evtcItem.SkillID;
        CompleteSkillInfoEvent(evtcItem);
    }

    internal void CompleteSkillInfoEvent(CombatItem evtcItem)
    {
        if (evtcItem.SkillID != SkillID)
        {
            throw new InvalidOperationException("Non matching buff id in BuffDataEvent complete method");
        }
        switch (evtcItem.IsStateChange)
        {
            case StateChange.SkillTiming:
                BuildFromSkillTiming(evtcItem);
                break;
            case StateChange.SkillInfo:
                BuildFromSkillInfo(evtcItem);
                break;
            default:
                throw new InvalidDataException("Invalid combat event in BuffDataEvent complete method");
        }
    }

    private unsafe void BuildFromSkillInfo(CombatItem evtcItem)
    {
        // 2 
        var time = evtcItem.Time;
        // 2
        var srcAgent = evtcItem.SrcAgent;

        Recharge    = *(float*)&time;
        Range0      = *((float*)&time + 1);
        Range1      = *(float*)&srcAgent;
        TooltipTime = *((float*)&srcAgent + 1);
    }

    private void BuildFromSkillTiming(CombatItem evtcItem)
    {
        _SkillTimings.Add(new SkillTiming(evtcItem));
    }

}
