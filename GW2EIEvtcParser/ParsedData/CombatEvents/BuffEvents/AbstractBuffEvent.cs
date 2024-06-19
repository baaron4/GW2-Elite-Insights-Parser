using GW2EIEvtcParser.EIData.BuffSimulators;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBuffEvent : AbstractTimeCombatEvent
    {
        public SkillItem BuffSkill { get; private set; }
        public long BuffID => BuffSkill.ID;
        //private long _originalBuffID;

        public AgentItem By { get; protected set; }
        public AgentItem CreditedBy => By.GetFinalMaster();

        public bool ToFriendly => IFF == IFF.Friend;
        public bool ToFoe => IFF == IFF.Foe;
        public bool ToUnknown => IFF == IFF.Unknown;

        public AgentItem To { get; protected set; }

        internal IFF IFF { get; }

        internal AbstractBuffEvent(CombatItem evtcItem, SkillData skillData) : base(evtcItem.Time)
        {
            BuffSkill = skillData.Get(evtcItem.SkillID);
            IFF = evtcItem.IFF;
        }

        internal AbstractBuffEvent(SkillItem buffSkill, long time, IFF iff) : base(time)
        {
            BuffSkill = buffSkill;
            IFF = iff;
        }

        internal void Invalidate(SkillData skillData)
        {
            if (BuffID != SkillIDs.NoBuff)
            {
                //_originalBuffID = BuffID;
                BuffSkill = skillData.Get(SkillIDs.NoBuff);
            }
        }

        internal abstract void UpdateSimulator(AbstractBuffSimulator simulator, bool forceStackType4ToBeActive);

        internal abstract void TryFindSrc(ParsedEvtcLog log);

        internal virtual bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator)
        {
            return BuffID != SkillIDs.NoBuff;
        }

        //internal abstract int CompareTo(AbstractBuffEvent abe);
    }
}
