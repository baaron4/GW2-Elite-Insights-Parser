namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractDamageEvent : AbstractTimeCombatEvent
    {
        public AgentItem From { get; }
        public AgentItem CreditedFrom => From.GetFinalMaster();
        public AgentItem To { get; }

        public SkillItem Skill { get; }
        public long SkillId => Skill.ID;
        private readonly ArcDPSEnums.IFF _iff;

        public bool ToFriendly => _iff == ArcDPSEnums.IFF.Friend;
        public bool ToFoe => _iff == ArcDPSEnums.IFF.Foe;
        public bool ToUnknown => _iff == ArcDPSEnums.IFF.Unknown;

        //private int _damage;
        public bool IsOverNinety { get; }
        public bool AgainstUnderFifty { get; }
        public bool IsMoving { get; }
        public bool IsFlanking { get; }

        internal AbstractDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem.Time)
        {
            From = agentData.GetAgent(evtcItem.SrcAgent);
            To = agentData.GetAgent(evtcItem.DstAgent);
            Skill = skillData.Get(evtcItem.SkillID);
            IsOverNinety = evtcItem.IsNinety > 0;
            AgainstUnderFifty = evtcItem.IsFifty > 0;
            IsMoving = evtcItem.IsMoving > 0;
            IsFlanking = evtcItem.IsFlanking > 0; 
            _iff = evtcItem.IFF;
        }

        public abstract bool ConditionDamageBased(ParsedEvtcLog log);
    }
}
