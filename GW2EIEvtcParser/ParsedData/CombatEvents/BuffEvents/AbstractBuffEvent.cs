using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBuffEvent : AbstractTimeCombatEvent
    {
        public SkillItem BuffSkill { get; private set; }
        public long BuffID => BuffSkill.ID;
        //private long _originalBuffID;

        protected AgentItem InternalBy { get; set; }
        public AgentItem By => InternalBy?.GetFinalMaster();

        public AgentItem ByMinion => InternalBy != null && InternalBy.Master != null ? InternalBy : null;

        public AgentItem To { get; protected set; }

        internal AbstractBuffEvent(CombatItem evtcItem, SkillData skillData) : base(evtcItem.Time)
        {
            BuffSkill = skillData.Get(evtcItem.SkillID);
        }

        internal AbstractBuffEvent(SkillItem buffSkill, long time) : base(time)
        {
            BuffSkill = buffSkill;
        }

        internal void Invalidate(SkillData skillData)
        {
            if (BuffID != ProfHelper.NoBuff)
            {
                //_originalBuffID = BuffID;
                BuffSkill = skillData.Get(ProfHelper.NoBuff);
            }
        }

        internal abstract void UpdateSimulator(AbstractBuffSimulator simulator);

        internal abstract void TryFindSrc(ParsedEvtcLog log);

        internal abstract bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs);

        internal abstract int CompareTo(AbstractBuffEvent abe);
    }
}
