using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public abstract class SkillEvent : TimeCombatEvent
{
    private int _isCondi = -1;
    public readonly AgentItem From;
    public AgentItem CreditedFrom => From.GetFinalMaster();
    public readonly AgentItem To;

    public readonly SkillItem Skill;
    public long SkillID => Skill.ID;
    private readonly IFF _iff;

    public bool ToFriendly => _iff == IFF.Friend;
    public bool ToFoe => _iff == IFF.Foe;
    public bool ToUnknown => _iff == IFF.Unknown;


    internal SkillEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem.Time)
    {
        From = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        To = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        Skill = skillData.Get(evtcItem.SkillID);
        _iff = evtcItem.IFF;
    }

    public bool ConditionDamageBased(ParsedEvtcLog log)
    {
        if (_isCondi == -1 && log.Buffs.BuffsByIDs.TryGetValue(SkillID, out var b))
        {
            _isCondi = b.Classification == Buff.BuffClassification.Condition ? 1 : 0;
        }
        return _isCondi == 1;
    }
}
