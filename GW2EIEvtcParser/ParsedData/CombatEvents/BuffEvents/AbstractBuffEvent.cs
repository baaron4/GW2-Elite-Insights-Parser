using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EIData.BuffSimulators;

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
            if (BuffID != SkillIDs.NoBuff)
            {
                //_originalBuffID = BuffID;
                BuffSkill = skillData.Get(SkillIDs.NoBuff);
            }
        }

        internal abstract void UpdateSimulator(AbstractBuffSimulator simulator);

        internal abstract void TryFindSrc(ParsedEvtcLog log);

        internal abstract bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator);

        //internal abstract int CompareTo(AbstractBuffEvent abe);
    }
}
