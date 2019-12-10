namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractCastEvent : AbstractTimeCombatEvent
    {
        // start item
        public SkillItem Skill { get; protected set; }
        public long SkillId => Skill.ID;
        public AgentItem Caster { get; }
        public int ExpectedDuration { get; protected set; }
        public bool UnderQuickness { get; protected set; }

        // end item
        public bool Interrupted { get; protected set; }
        public bool FullAnimation { get; protected set; }
        public bool ReducedAnimation { get; protected set; }
        public int Duration { get; protected set; }

        public AbstractCastEvent(CombatItem startEvtcItem, AgentData agentData, SkillData skillData) : base(startEvtcItem.Time)
        {
            Skill = skillData.Get(startEvtcItem.SkillID);
            Caster = agentData.GetAgent(startEvtcItem.SrcAgent);
            UnderQuickness = startEvtcItem.IsActivation == ParseEnum.Activation.Quickness;
            ExpectedDuration = startEvtcItem.Value;
        }

        public AbstractCastEvent(long time, SkillItem skill, AgentItem caster) : base(time)
        {
            Skill = skill;
            Caster = caster;
        }
    }
}
