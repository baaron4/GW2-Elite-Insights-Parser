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
        public int ActualDuration { get; protected set; }

        public AbstractCastEvent(CombatItem startEvtcItem, AgentData agentData, SkillData skillData, long offset) : base(startEvtcItem.LogTime, offset)
        {
            Skill = skillData.Get(startEvtcItem.SkillID);
            Caster = agentData.GetAgent(startEvtcItem.SrcAgent);
            UnderQuickness = startEvtcItem.IsActivation == ParseEnum.EvtcActivation.Quickness;
            ExpectedDuration = startEvtcItem.Value;
        }

        public AbstractCastEvent(long time, SkillItem skill, AgentItem caster) : base(time, 0)
        {
            Skill = skill;
            Caster = caster;
        }
    }
}
