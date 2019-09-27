using LuckParser.EIData;

namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractBuffEvent : AbstractCombatEvent
    {
        public SkillItem BuffSkill { get; private set; }
        public long BuffID => BuffSkill.ID;
        private long _originalBuffID;
        public AgentItem By { get; protected set; }
        public AgentItem ByMinion { get; protected set; }
        public AgentItem To { get; protected set; }

        public AbstractBuffEvent(CombatItem evtcItem, SkillData skillData, long offset) : base(evtcItem.LogTime, offset)
        {
#if DEBUG
            OriginalCombatEvent = evtcItem;
#endif
            BuffSkill = skillData.Get(evtcItem.SkillID);
        }

        public AbstractBuffEvent(SkillItem buffSkill, long time) : base(time, 0)
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

        public abstract void UpdateSimulator(BuffSimulator simulator);

        public abstract void TryFindSrc(ParsedLog log);

        public abstract bool IsBoonSimulatorCompliant(long fightEnd);

        public abstract int CompareTo(AbstractBuffEvent abe);
    }
}
