using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public abstract class SkillEvent : TimeCombatEvent
{
    private int _isCondi = -1;
    public AgentItem From { get; protected set; }
    public AgentItem CreditedFrom => From.GetFinalMaster();
    public AgentItem To { get; protected set; }

    public readonly SkillItem Skill;
    public long SkillID => Skill.ID;
    private readonly IFF _iff;

    public bool ToFriendly => _iff == IFF.Friend;
    public bool ToFoe => _iff == IFF.Foe;
    public bool ToUnknown => _iff == IFF.Unknown;

    public readonly bool IsOverNinety;
    public readonly bool AgainstUnderFifty;
    public readonly bool IsMoving;
    public readonly bool AgainstMoving;
    public readonly bool IsFlanking;
    public bool AgainstDowned { get; protected set; }


    internal SkillEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem.Time)
    {
        From = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        To = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        Skill = skillData.Get(evtcItem.SkillID);
        _iff = evtcItem.IFF;

        IsOverNinety = evtcItem.IsNinety > 0;
        AgainstUnderFifty = evtcItem.IsFifty > 0;
        IsMoving = (evtcItem.IsMoving & 1) > 0;
        AgainstMoving = (evtcItem.IsMoving & 2) > 0;
        IsFlanking = evtcItem.IsFlanking > 0;
    }

    public bool ConditionDamageBased(ParsedEvtcLog log)
    {
        if (_isCondi == -1 && log.Buffs.BuffsByIDs.TryGetValue(SkillID, out var b))
        {
            _isCondi = b.Classification == Buff.BuffClassification.Condition ? 1 : 0;
        }
        return _isCondi == 1;
    }

    internal abstract double GetValue();
}
