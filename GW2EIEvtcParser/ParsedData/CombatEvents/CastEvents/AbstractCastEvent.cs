namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractCastEvent : AbstractTimeCombatEvent
    {

        public enum AnimationStatus { Unknown, Reduced, Iterrupted, Full};

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

        public AbstractCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData) : base(startItem.Time)
        {
            Skill = skillData.Get(startItem.SkillID);
            Caster = agentData.GetAgent(startItem.SrcAgent);
        }

        public AbstractCastEvent(long time, SkillItem skill, AgentItem caster) : base(time)
        {
            Skill = skill;
            Caster = caster;
        }
    }
}
