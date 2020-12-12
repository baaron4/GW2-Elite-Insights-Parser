namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractCastEvent : AbstractTimeCombatEvent
    {

        public enum AnimationStatus { Unknown, Reduced, Interrupted, Full, Instant };

        // start item
        public SkillItem Skill { get; protected set; }
        public long SkillId => Skill.ID;
        public AgentItem Caster { get; }

        public AnimationStatus Status { get; protected set; } = AnimationStatus.Unknown;
        public int SavedDuration { get; protected set; }

        public int ExpectedDuration { get; protected set; }

        public int ActualDuration { get; protected set; }

        public long EndTime => Time + ActualDuration;

        public double Acceleration { get; protected set; } = 0;

        internal AbstractCastEvent(CombatItem baseItem, AgentData agentData, SkillData skillData) : base(baseItem.Time)
        {
            Skill = skillData.Get(baseItem.SkillID);
            Caster = agentData.GetAgent(baseItem.SrcAgent);
        }

        internal AbstractCastEvent(long time, SkillItem skill, AgentItem caster) : base(time)
        {
            Skill = skill;
            Caster = caster;
        }
    }
}
