namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractDamageEvent : AbstractTimeCombatEvent
    {
        public AgentItem From { get; }
        public AgentItem To { get; }

        public SkillItem Skill { get; }
        public long SkillId => Skill.ID;
        public ParseEnum.IFF IFF { get; }

        private int _damage;
        public int Damage { get; protected set; }
        public int ShieldDamage { get; protected set; }
        public bool IsOverNinety { get; }
        public bool AgainstUnderFifty { get; }
        public bool IsMoving { get; }
        public bool IsFlanking { get; }
        public bool HasHit { get; protected set; }
        public bool HasCrit { get; protected set; }
        public bool HasGlanced { get; protected set; }
        public bool IsBlind { get; protected set; }
        public bool IsAbsorbed { get; protected set; }
        public bool HasInterrupted { get; protected set; }
        public bool HasDowned { get; protected set; }
        public bool HasKilled { get; protected set; }
        public bool IsBlocked { get; protected set; }
        public bool IsEvaded { get; protected set; }

        public AbstractDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem.LogTime, offset)
        {
            From = agentData.GetAgent(evtcItem.SrcAgent);
            To = agentData.GetAgent(evtcItem.DstAgent);
            Skill = skillData.Get(evtcItem.SkillID);
            IsOverNinety = evtcItem.IsNinety > 0;
            AgainstUnderFifty = evtcItem.IsFifty > 0;
            IsMoving = evtcItem.IsMoving > 0;
            IsFlanking = evtcItem.IsFlanking > 0;
            IFF = evtcItem.IFF;
        }

        public void NegateDamage()
        {
            _damage = Damage;
            Damage = 0;
        }

        public abstract bool IsCondi(ParsedLog log);
    }
}
