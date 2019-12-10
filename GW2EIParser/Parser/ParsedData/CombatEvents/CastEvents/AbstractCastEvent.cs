namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractCastEvent : AbstractTimeCombatEvent
    {

        public enum AnimationStatus { UNKNOWN, REDUCED, INTERRUPTED, FULL};

        // start item
        public SkillItem Skill { get; protected set; }
        public long SkillId => Skill.ID;
        public AgentItem Caster { get; }

        public bool UnderQuickness { get; protected set; }

        public AnimationStatus Status { get; protected set; } = AnimationStatus.UNKNOWN;
        public int SavedDuration { get; protected set; }


        public int ExpectedDuration { get; protected set; }

        public int ActualDuration { get; protected set; }

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
