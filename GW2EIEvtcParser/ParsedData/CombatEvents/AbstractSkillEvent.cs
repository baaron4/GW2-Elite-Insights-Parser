using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractSkillEvent : AbstractTimeCombatEvent
    {
        private int _isCondi = -1;
        public AgentItem From { get; }
        public AgentItem CreditedFrom => From.GetFinalMaster();
        public AgentItem To { get; }

        public SkillItem Skill { get; }
        public long SkillId => Skill.ID;
        private readonly ArcDPSEnums.IFF _iff;

        public bool ToFriendly => _iff == ArcDPSEnums.IFF.Friend;
        public bool ToFoe => _iff == ArcDPSEnums.IFF.Foe;
        public bool ToUnknown => _iff == ArcDPSEnums.IFF.Unknown;


        internal AbstractSkillEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem.Time)
        {
            From = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
            To = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
            Skill = skillData.Get(evtcItem.SkillID);
            _iff = evtcItem.IFF;
        }

        public bool ConditionDamageBased(ParsedEvtcLog log)
        {
            if (_isCondi == -1 && log.Buffs.BuffsByIds.TryGetValue(SkillId, out Buff b))
            {
                _isCondi = b.Classification == Buff.BuffClassification.Condition ? 1 : 0;
            }
            return _isCondi == 1;
        }
    }
}
