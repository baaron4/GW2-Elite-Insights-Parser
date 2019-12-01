using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractBuffEvent : AbstractTimeCombatEvent
    {
        public SkillItem BuffSkill { get; private set; }
        public long BuffID => BuffSkill.ID;
        private long _originalBuffID;
        public AgentItem By { get; protected set; }
        public AgentItem ByMinion { get; protected set; }
        public AgentItem To { get; protected set; }

        public AbstractBuffEvent(CombatItem evtcItem, SkillData skillData) : base(evtcItem.Time)
        {
            BuffSkill = skillData.Get(evtcItem.SkillID);
        }

        public AbstractBuffEvent(SkillItem buffSkill, long time) : base(time)
        {
            BuffSkill = buffSkill;
        }

        public void Invalidate(SkillData skillData)
        {
            if (BuffID != ProfHelper.NoBuff)
            {
                _originalBuffID = BuffID;
                BuffSkill = skillData.Get(ProfHelper.NoBuff);
            }
        }

        public abstract void UpdateSimulator(AbstractBuffSimulator simulator);

        public abstract void TryFindSrc(ParsedLog log);

        public abstract bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs);

        public abstract int CompareTo(AbstractBuffEvent abe);
    }
}
