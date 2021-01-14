using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBuffEvent : AbstractTimeCombatEvent
    {
        public SkillItem BuffSkill { get; private set; }
        public long BuffID => BuffSkill.ID;
        //private long _originalBuffID;

        public AgentItem By { get; protected set; }
        public AgentItem CreditedBy => By.GetFinalMaster();

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
            if (BuffID != Buff.NoBuff)
            {
                //_originalBuffID = BuffID;
                BuffSkill = skillData.Get(Buff.NoBuff);
            }
        }

        internal abstract void UpdateSimulator(AbstractBuffSimulator simulator);

        internal abstract void TryFindSrc(ParsedEvtcLog log);

        internal abstract bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs);

        internal abstract int CompareTo(AbstractBuffEvent abe);
    }
}
